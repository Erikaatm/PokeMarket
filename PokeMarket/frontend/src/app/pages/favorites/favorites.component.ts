import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { MenuComponent } from '../../components/menu/menu.component';
import { CardResponse } from '../../models/DTOs/card-response';
import { PagedResult } from '../../models/DTOs/pagedresult';
import { FavoriteService } from '../../services/favorite/favorite.service';
import { FormsModule } from '@angular/forms';
import { CommonModule, isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'app-favorites',
  imports: [MenuComponent, FormsModule, CommonModule],
  templateUrl: './favorites.component.html',
  styleUrl: './favorites.component.css',
})
export class FavoritesComponent {
  // Declaración de variables
  favorites: CardResponse[] = [];
  currentPage = 1;

  // Paginado para las cartas
  pagedResult: PagedResult<CardResponse> = {
    currentPage: 1,
    pageSize: 10,
    totalPages: 1,
    totalCount: 0,
    items: [],
  };

  // Filtros y ordenacion
  selectedRarity: string = 'todos'; // Este es el valor inicial
  selectedOrder: string = 'fecha-desc';
  pokemonType: string = '';
  collection: string = '';
  pokemonName: string = '';
  tagName: string = '';
  errorMessage: string = '';
  orderBy: string = '';

  constructor(
    private favoriteService: FavoriteService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      document.body.className = 'fondo-favoritos';
      this.loadFavorites();
    }
  }

  loadFavorites(): void {
    // Convertimos 'todos' en filtro vacío para que no lo mande al backend
    const rarityFilter =
      this.selectedRarity === 'todos' ? '' : this.selectedRarity;

    this.favoriteService
      .getMyFavorites(
        this.currentPage,
        this.collection,
        this.pokemonType,
        this.pokemonName,
        rarityFilter,
        this.tagName,
        this.selectedOrder,
      )
      .subscribe({
        next: (response) => {
          this.pagedResult = response;
          this.favorites = response.items;
          this.errorMessage = '';
        },
        error: (error) => {
          // Ponemos el error en el div
          if (error.error && error.error.message) {
            this.errorMessage = error.error.message;
          } else {
            this.errorMessage = 'se ha producido un error.';
          }
          this.favorites = [];
        },
      });
  }
  // Método para cuando se cambia el filtro o el orden
  onFilterOrOrderChange(): void {
    this.currentPage = 1;
    this.loadFavorites();
  }

  // Paginación
  nextPage(): void {
    if (this.pagedResult && this.currentPage < this.pagedResult.totalPages) {
      this.currentPage++;
      this.loadFavorites();
    }
  }

  prevPage(): void {
    if (this.pagedResult && this.currentPage > 1) {
      this.currentPage--;
      this.loadFavorites();
    }
  }

  // Método para quitar cartas de favoritos
  removeFavorite(cardId:number): void {
    this.favoriteService.removeFavorite(cardId).subscribe(() => {
        this.loadFavorites();
      });
  }
}
