import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, from, map, Observable } from 'rxjs';
import { CartItemResponse } from '../../models/DTOs/cart-item-response';
import { AddToCartRequest } from '../../models/DTOs/add-to-cart-request';
import { environment } from '../../../environments/environment';
import { AuthService } from '../auth/auth.service';
import { CardService } from '../card/card.service';
import { UpdateCartItemRequest } from '../../models/DTOs/update-cart-item-request';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  private apiURL = `${environment.apiUrl}/Cart`;
  private cartChanged = new BehaviorSubject<void>(undefined);
  cartChanged$ = this.cartChanged.asObservable();

  constructor(
    private http: HttpClient,
    private authService: AuthService,
    private cardService: CardService
  ) {}

  // Método para añadir cartas al carrito
  addCardsToCart(
    addToCartRequest: AddToCartRequest
  ): Observable<{ message: string }> | void {
    if (this.authService.isLogged) {
      return this.http.post<{ message: string }>(
        `${this.apiURL}`,
        addToCartRequest
      );
    } else {
      // Lógica del localStorage si no está logeado
      const localCart = this.getLocalCart();

      // Verificamos sin la carta que quiere añadir el usuario está ya en el carrito local
      const existsLocalCart = localCart.find(
        (item) => item.cardId === addToCartRequest.cardId
      );

      // Si existe se añade 1 a la cantidad
      if (existsLocalCart) {
        existsLocalCart.quantity += addToCartRequest.quantity;
      } else {
        // Si no, se añade la carta
        localCart.push({
          cardId: addToCartRequest.cardId,
          quantity: addToCartRequest.quantity,
        });
      }

      // Guardamos el carrito en el localStorage
      this.saveLocalCart(localCart);

      // Para actualizar el carrito en toda la app
      this.cartChanged.next();
    }
  }

  // Método para obtener las carrito del carrito tanto back como localStorage
  getCart(): Observable<{ carrito: CartItemResponse[]; total: number }> {
    // Comprobamos que el usuario está loggeado
    if (this.authService.isLogged) {
      // Usamos carrito back
      return this.http.get<{ carrito: CartItemResponse[]; total: number }>(
        `${this.apiURL}`
      );
    } else {
      // Usamos carrito local
      const localCart = this.getLocalCart();

      // Generamos un array de promesas para hacer peticiones en paralelo y nos devulva las cartas del back
      const promisesCards = localCart.map(async (item) => {
        // Se guardan las cartas en una variable
        const card = await this.cardService
          .getCardById(item.cardId)
          .toPromise();
        console.log('CARD LOCAL:', card); // AÑADE ESTO

        // Devolvemos la respuesta de CartItemsResponse
        return {
          id: 0,
          cardId: item.cardId,
          pokemonName: card?.pokemonName,
          imageUrl: card?.imageUrl,
          price: card?.price,
          quantity: item.quantity,
        } as CartItemResponse;
      });

      // Luego usamos from() para que la promesa se convierta en un observable que es lo que necesitamos
      return from(Promise.all(promisesCards)).pipe(
        // Transformamos el valor que sale de nuestras promesas
        map((carrito: CartItemResponse[]) => {
          // Calculamos el precio total del carrito
          // Con reduce reocrremos todo el array de las cartas sumando el precio de cada una
          // acc es un acumulador el cual empieza en 0
          const total = carrito.reduce(
            (acc, item) => acc + item.price * item.quantity,
            0
          );

          // Devolvemos el carrito y el total
          return {
            carrito,
            total,
          };
        })
      );
    }
  }

  // Método para eliminar una carta del carrito
  deleteFromCart(cardId: number): Observable<any> {
    return this.http.delete(`${this.apiURL}/${cardId}`);
  }

  // Método para actualizar una carta del carrito
  updateCartItem(
    updateCartItemRequest: UpdateCartItemRequest
  ): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(
      `${this.apiURL}`,
      updateCartItemRequest
    );
  }

  // Método para fusionar el carrito del localStorage y backend
  mergeLocalWithBackCart(): void {
    // Obtenemos el carrito local
    const localCart = this.getLocalCart();

    // Si no hay cartas en el localStorage no hacemos nada
    if (localCart.length === 0) {
      return;
    }

    // Por el contrario, recorremos cada carta que este en el carrito local y la enviamos al backend
    localCart.forEach((item) => {
      this.http.post(`${this.apiURL}`, item).subscribe({
        next: () => {
          console.log(
            `Carta ${item.cardId} fusionada con el carrito del backend.`
          );
        },
        error: (error) => {
          console.error(`Error al fusionar carta ${item.cardId}:`, error);
        },
      });
    });

    // Limpiamos el carrito local después de fusionar
    this.clearLocalCart();
  }

  // Método para borrar el carrito localStorage
  clearLocalCart(): void {
    localStorage.removeItem('cart');
    this.cartChanged.next();
  }

  // Carrito local para los usuarios que no están logeados
  private getLocalCart(): { cardId: number; quantity: number }[] {
    const cart = localStorage.getItem('cart');

    if (!cart) return [];

    const parsed = JSON.parse(cart);

    // Si tiene .items, devolvemos eso
    if (Array.isArray(parsed)) {
      return parsed;
    } else if (parsed.items && Array.isArray(parsed.items)) {
      return parsed.items;
    }

    return [];
  }

  // Método para guardar el carrito en el localStorage
  private saveLocalCart(cart: { cardId: number; quantity: number }[]): void {
    localStorage.setItem('cart', JSON.stringify(cart));
  }

  // Método para eliminar una carta del carrito por su ID
  removeCardFromLocalCart(cardId: number): void {
    // Quitamos la carta del localStorage
    const localCart = this.getLocalCart();

    // Filtramos el array de cartas que hay en el carrito local quitando esa carta con el ID
    const updatedCart = localCart.filter((item) => item.cardId !== cardId);

    // Guardamos el nuevo carrito
    this.saveLocalCart(updatedCart);
    this.cartChanged.next();
  }

  // Método para actualizar las cartas en el localcarrito
  updateItemLocalCart(cardId: number, newQuantity: number): void {
    // Obtenemos el carrito
    const localCart = this.getLocalCart();

    // Buscamos la carta en el carrito local
    const item = localCart.find((item) => item.cardId == cardId);

    // Si loe ncontramos, actualizamos la cantidad
    if (item) {
      if (newQuantity < 1) {
        // Si la cantidad es menor a 1, eliminamos la carta del carrito
        this.removeCardFromLocalCart(cardId);
      } else {
        item.quantity = newQuantity;
        this.saveLocalCart(localCart);
      }
    }
    this.cartChanged.next();
  }

  // Método para vaciar el carrito
  clearCart(): void | Observable<any> {
    // comprobamos si el usuario está logeado
    if (this.authService.isLogged) {
      return this.http.delete(`${this.apiURL}/clear`);
    } else {
      this.clearLocalCart();
    }
  }
}
