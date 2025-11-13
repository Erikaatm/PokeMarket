import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { MenuComponent } from '../../components/menu/menu.component';
import { CartItemResponse } from '../../models/DTOs/cart-item-response';
import { CartService } from '../../services/cart/cart.service';
import { FormsModule } from '@angular/forms';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { AuthService } from '../../services/auth/auth.service';
import { Router } from '@angular/router';
import { UpdateCartItemRequest } from '../../models/DTOs/update-cart-item-request';

@Component({
  selector: 'app-cart',
  imports: [MenuComponent, FormsModule, CommonModule],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.css',
})
export class CartComponent {
  // Declaración de variables
  cartItems: CartItemResponse[] = [];
  total: number = 0;
  errorMessage: string = '';

  constructor(
    private cartService: CartService,
    private authService: AuthService,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  // Método para que la página cargue automáticamente
  ngOnInit(): void {
     if (isPlatformBrowser(this.platformId)) {
    document.body.className = 'cart-body';
    this.loadCart();

    // Nos suscribimos a los cambios del carrito
    this.cartService.cartChanged$.subscribe(() => {
      this.loadCart();
    });
  }
  }

  loadCart(): void {
    this.cartService.getCart().subscribe({
      next: (response) => {
        // Guardamos las cartas obtenidas del back
        this.cartItems = response.carrito;

        // Guardamos el precio total de nuestro carrito
        this.total = response.total;

        // Eliminamos el mensaje de error
        this.errorMessage = '';
      },
      error: (error) => {
        // Ponemos el error en el div
        if (error.error && error.error.message) {
          this.errorMessage = error.error.message;
        } else {
          this.errorMessage = 'se ha producido un error.';
        }
      },
    });
  }

  // Método para redirigir a partir del boton finalizar compra
  finalizarCompra(): void {
    if (this.authService.isLogged) {
      // Si el usuario está logeado le redirigimos a la pagina de compra normal
      this.router.navigate(['/compra-realizada']);
    } else {
      // Si el usuario no está logeado se guarda en el localStorage la intención y redirigimos al login
      localStorage.setItem('redirectAfterLogin', '/compra-realizada');
      this.router.navigate(['/login']);
    }
  }

  // Método para reducir la cantidad de una carta en el carrito
  decreaseQuantity(item: CartItemResponse): void {
    // Obtenemos la cantidad
    const newQuantity = item.quantity - 1;

    // Comprobamos si es menor que 1
    if (newQuantity < 1) {
      // Si lo es, eliminamos la carta
      this.removeItem(item.cardId);
    } else {
      // si no, actualizamos la cantidad
      this.updateQuantity(item, newQuantity);
    }
  }

  // Método para aumentar la cantidad de una carta en el carrito
  increaseQuantity(item: CartItemResponse): void {
    // Obtenemos la cantidad
    const newQuantity = item.quantity + 1;
    if (newQuantity <= 100) {
      this.updateQuantity(item, newQuantity);
    }
  }

  // Método para cambiar la cantidad según se escriba en el input
  quantityChange(item: CartItemResponse, newQuantity: number): void {
    // Comprobamos si es un número válido
    if (isNaN(newQuantity)) return;

    if (newQuantity < 1) {
      this.removeItem(item.cardId);
    } else if (newQuantity > 100) {
      this.updateQuantity(item, 100);
      item.quantity = 100;
    } else {
      this.updateQuantity(item, newQuantity);
    }
  }

  // Método para actualizar la cantidad de las cartas
  updateQuantity(item: CartItemResponse, quantity: number): void {
    // Comprobamos si el usuario está logeado o no
    if (this.authService.isLogged) {
      const request: UpdateCartItemRequest = {
        cardId: item.cardId,
        quantity: quantity,
      };

      // Llamamos al método
      this.cartService.updateCartItem(request).subscribe(() => {
        this.loadCart();
      });
    } else {
      this.cartService.updateItemLocalCart(item.cardId, quantity);
      this.loadCart();
    }
  }

  // Método para eliminar una carta del carrito
  removeItem(cardId: number): void {
    // Comprobamos si el usuario está logeado o no
    if (this.authService.isLogged) {
      // Llamamos al método
      this.cartService.deleteFromCart(cardId).subscribe(() => {
        this.loadCart();
      });
    } else {
      this.cartService.removeCardFromLocalCart(cardId);
      this.loadCart();
    }
  }

  // Método para vaciar el carrito
  clearCart(): void {
    const result = this.cartService.clearCart();

    // Solo si devuelve observable, nos suscribimos
    if (result && result.subscribe) {
      result.subscribe(() => this.loadCart());
    } else {
      this.loadCart(); // Si era local, recargamos igual
    }
  }
}
