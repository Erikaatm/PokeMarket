import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { CardResponse } from '../../../../models/DTOs/card-response';
import { GradeCardRequest } from '../../../../models/DTOs/grade-card-request';
import { GradingResponse } from '../../../../models/DTOs/grading-response';
import { PagedResult } from '../../../../models/DTOs/pagedresult';
import { GradingService } from '../../../../services/grading/grading.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-requested',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './requested.component.html',
  styleUrl: './requested.component.css',
})
export class RequestedComponent {
  // Variables para filtros
  pokemonType: string = '';
  collection: string = '';
  pokemonName: string = '';
  rarity: string = 'todos';
  gradedBy: string = '';
  selectedOrder: string = 'fecha-desc';
  errorMessage: string = '';

  // Paginación
  currentPage: number = 1;
  pagedResult: PagedResult<CardResponse> = {
    currentPage: 1,
    pageSize: 10,
    totalPages: 1,
    totalCount: 0,
    items: [],
  };

  // Lista de gradeos obtenidos
  gradingInputVisible: number | null = null; // ID de la carta para la que se ve el input
  gradeToSubmit: { [cardId: number]: number } = {}; // Guarda la nota escrita por carta
  successMessage: string = '';
  cards: CardResponse[] = [];
  selectedTab = 'requested';

  constructor(
    private gradingService: GradingService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  // Método para que cargue directamente
  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.loadRequestedCards();
    }
  }

  // Método para cuando se cambia el filtro o el orden
  onFilterOrOrderChange(): void {
    this.currentPage = 1;
    this.loadRequestedCards();
  }

  nextPage(): void {
    if (this.pagedResult && this.currentPage < this.pagedResult.totalPages) {
      this.currentPage++;
      this.loadRequestedCards();
    }
  }

  prevPage(): void {
    if (this.pagedResult && this.currentPage > 1) {
      this.currentPage--;
      this.loadRequestedCards();
    }
  }

  // Método para mostrar o esconder el input de las notas
  toggleGradingInput(cardId: number): void {
    if (this.gradingInputVisible === cardId) {
      this.gradingInputVisible = null;
    } else {
      this.gradingInputVisible = cardId;
    }
  }

  // Método para gradear cartas
  gradeCard(cardId: number): void {
    // Guardamos la nota que hemos puesto
    const grade = this.gradeToSubmit[cardId];

    // Comprobamos que es válida
    if (!grade || grade < 1 || grade > 10) {
      alert('La nota debe estar entre 1 y 10.');
      return;
    }

    const request: GradeCardRequest = {
      cardId: cardId,
      grade: grade,
    };

    this.gradingService.gradeCard(request).subscribe({
      next: () => {
        this.successMessage = 'Carta gradeada con éxito.';
        this.errorMessage = '';
        this.loadRequestedCards(); // Volvemos a cargar para actualizar el estado
        this.gradingInputVisible = null; // Ocultamos el input
      },
      error: (error) => {
        this.errorMessage =
          error.error?.message || 'Error al aceptar el gradeo.';
      },
    });
  }

  // Método para rechazar ubn gradeo
  rejectGrading(cardId: number): void {
    this.gradingService.rejectGrading(cardId).subscribe({
      next: (res) => {
        this.successMessage = res.message;
        this.errorMessage = '';
        this.loadRequestedCards();
      },
      error: (err) => {
        this.successMessage = '';
        this.errorMessage =
          err.error?.message || 'Error al rechazar el gradeo.';
      },
    });
  }

  // Método para eliminar un gradeo
  deleteGrading(gradingId: number): void {
    this.gradingService.deleteGrading(gradingId).subscribe({
      next: (res) => {
        this.successMessage = res.message;
        this.loadRequestedCards();
      },
      error: (err) => {
        this.errorMessage =
          err.error?.message || 'Error al eliminar el gradeo.';
      },
    });
  }

  // Método para obtener los gradeos solicitados
  loadRequestedCards(): void {
    this.gradingService
      .getRequestedCards(
        this.currentPage,
        this.collection,
        this.pokemonType.trim(),
        this.pokemonName.trim(),
        this.rarity === 'todos' ? '' : this.rarity,
        this.selectedOrder
      )
      .subscribe({
        next: (response) => {
          this.pagedResult = response;
          this.cards = response.items;
        },
        error: (error) => {
          this.errorMessage =
            error.error?.message || 'Ha ocurrido un error inesperado.';
        },
      });
  }
}
