import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { MenuComponent } from '../../components/menu/menu.component';
import { PagedResult } from '../../models/DTOs/pagedresult';
import { CardService } from '../../services/card/card.service';
import { FormsModule } from '@angular/forms';
import { CardResponse } from '../../models/DTOs/card-response';
import { Router, RouterLink } from '@angular/router';
import { CartService } from '../../services/cart/cart.service';
import { AddToCartRequest } from '../../models/DTOs/add-to-cart-request';
import { AuthService } from '../../services/auth/auth.service';
import { FavoriteService } from '../../services/favorite/favorite.service';

@Component({
  selector: 'app-surging-sparks',
  imports: [MenuComponent, FormsModule, CommonModule, RouterLink],
  templateUrl: './surging-sparks.component.html',
  styleUrl: './surging-sparks.component.css',
})
export class SurgingSparksComponent {
  // Declaramos
  nombrePlaceHolder: string = 'Filtrar por nombre...';
  coleccionPlaceHolder: string = 'Filtrar por colección...';
  tipoPlaceHolder: string = 'Filtrar por tipo...';
  tagPlaceHolder: string = 'Filtrar por tag...';
  selectedCard: CardResponse | null = null;
  showCartMessage: boolean = false;
  favoritesLoaded: boolean = false;

  // Paginado para las cartas
  pagedResult: PagedResult<CardResponse> = {
    currentPage: 1,
    pageSize: 10,
    totalPages: 1,
    totalCount: 0,
    items: [],
  };

  // Lista de cartas
  cards: CardResponse[] = [];
  currentPage: number = 1;

  // Lista de cartas filtradas
  filteredCards: CardResponse[] = [];

  // Lista de cartas favoritas
  myFavoriteCardsIds: number[] = [];

  selectedRarity: string = 'todos'; // Este es el valor inicial
  selectedOrder: string = 'fecha-desc';
  pokemonType: string = '';
  collection: string = 'Surging Sparks';
  pokemonName: string = '';
  tagName: string = '';
  errorMessage: string = '';

  // Inyectamos el cardService para poder usarlo más tarde
  constructor(
    private cardService: CardService,
    @Inject(PLATFORM_ID) private platformId: Object, // Para poder cambiar el body segun la pagina que sea
    private cartService: CartService,
    private authService: AuthService,
    private router: Router,
    private favoriteService: FavoriteService
  ) {}

  // Método que se ejecuta automáticamente cuando se carga el componente
  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      document.body.className = 'fondo-surging-sparks';
      this.loadCards();

      this.loadFavoriteCardIds();
    }
  }

  // Método para llamar al cardService para obtener las cartas
  loadCards(): void {
    // Declaramos la función, devuelve void porque no retorna ningún valor
    this.cardService
      .getCards(
        this.currentPage,
        this.pokemonType.trim(),
        this.collection.trim(),
        this.pokemonName.trim(),
        this.selectedRarity === 'todos' ? '' : this.selectedRarity, // Por si no quieren filtrar
        this.tagName.trim(),
        this.selectedOrder
      )
      .subscribe({
        // Llamamos al CardService y a su método getCards para todas las cartas
        next: (pagedResult) => {
          // Ejecutamos después de que el back responda correctamente y recibe el array de cartas
          this.pagedResult = pagedResult; // Guardamos las cartas que nos ha devuelto el backend en nuestra variable interna

          this.cards = pagedResult.items;

          this.loadFavoriteCardIds();

          console.log('Cartas cargadas: ', this.cards); // Lo mostramos por consola para depurar
        },
        error: (error) => {
          // Ponemos el error en el div
          if (error.error && error.error.message) {
            this.errorMessage = error.error.message;
          } else {
            this.errorMessage = 'se ha producido un error.';
          }

          // Limpiamos las cartas
          this.cards = [];
        },
      });
  }

  // Método para añadir las cartas al carrito
  addToCart(card: CardResponse): void {
    const addToCartRequest: AddToCartRequest = {
      cardId: card.id,
      quantity: 1,
    };

    const result = this.cartService.addCardsToCart(addToCartRequest);

    if (result) {
      result.subscribe({
        next: () => {
          this.selectedCard = card;
          this.showCartMessage = true;
        },
      });
    } else {
      // Si no hay result, es porque se guardó en localStorage
      this.selectedCard = card;
      this.showCartMessage = true;
    }
  }

  // Método para cuando se cambia el filtro o el orden
  onFilterOrOrderChange(): void {
    this.currentPage = 1;
    this.loadCards();
  }

  // Paginación
  nextPage(): void {
    if (this.pagedResult && this.currentPage < this.pagedResult.totalPages) {
      this.currentPage++;
      this.loadCards();
    }
  }

  prevPage(): void {
    if (this.pagedResult && this.currentPage > 1) {
      this.currentPage--;
      this.loadCards();
    }
  }

  // Método para saber si una carta está en favoritos
  isFavorite(cardId: number): boolean {
    return this.myFavoriteCardsIds.includes(cardId);
  }

  // Método para añadir y quitar de favoritos
  toggleFavorite(cardId: number): void {
    if (!this.authService.isLogged) {
      this.router.navigate(['/login']);
      return;
    }

    // Comprobamos si las cartas están en favorito
    if (this.isFavorite(cardId)) {
      // Si está la quitamos
      this.favoriteService.removeFavorite(cardId).subscribe({
        next: () => {
          // Buscamos en la lista de las cartas favoritas
          this.myFavoriteCardsIds = this.myFavoriteCardsIds.filter(
            (id) => id !== cardId
          );
        },
      });
    } else {
      // Si no la añadimos
      this.favoriteService.addFavorite({ cardId }).subscribe({
        next: () => {
          this.myFavoriteCardsIds.push(cardId);
        },
      });
    }
  }

  loadFavoriteCardIds(): void {
    if (this.authService.isLogged) {
      this.favoriteService.getMyFavoriteIds().subscribe({
        next: (res) => {
          this.myFavoriteCardsIds = res;
          this.favoritesLoaded = true;
        },
        error: () => {
          this.myFavoriteCardsIds = [];
          this.favoritesLoaded = true;
        },
      });
    } else {
      this.favoritesLoaded = true;
    }
  }
}
