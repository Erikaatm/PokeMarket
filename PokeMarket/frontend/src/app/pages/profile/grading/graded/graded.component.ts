import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { CardResponse } from '../../../../models/DTOs/card-response';
import { GradeCardRequest } from '../../../../models/DTOs/grade-card-request';
import { GradingResponse } from '../../../../models/DTOs/grading-response';
import { PagedResult } from '../../../../models/DTOs/pagedresult';
import { GradingService } from '../../../../services/grading/grading.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-graded',
  imports: [FormsModule, CommonModule],
  templateUrl: './graded.component.html',
  styleUrl: './graded.component.css'
})
export class GradedComponent {

  // Variables para filtros
  pokemonType: string = '';
  collection: string = '';
  pokemonName: string = '';
  rarity: string = 'todos';
  gradedBy: string = '';
  selectedOrder: string = 'fecha-desc';
  tagName: string = '';
  errorMessage: string = '';

  // Paginación
  currentPage: number = 1;

  pagedResult: PagedResult<GradingResponse> = {
    currentPage: 1,
    pageSize: 10,
    totalPages: 1,
    totalCount: 0,
    items: [],
  };

  // Lista de gradeos obtenidos
  gradings: GradingResponse[] = [];
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
      this.loadGradings();
    }
  }

  // Método para cargar los gradeos
  loadGradings(): void {
    this.gradingService
      .getAllGradings(
        this.currentPage,
        this.pokemonType.trim(),
        this.collection.trim(),
        this.pokemonName.trim(),
        this.rarity === 'todos' ? '' : this.rarity,
        this.selectedOrder,
        this.gradedBy.trim()
      )
      .subscribe({
        next: (response) => {
          this.pagedResult = response;
          this.gradings = response.items;
        },
        error: (err) => {
          if (err.error && err.error.message) {
            this.errorMessage = err.error.message;
          } else {
            this.errorMessage =
              'Se ha producido un error al cargar los gradeos.';
          }
          this.gradings = [];
        },
      });
  }

  // Método para cuando se cambia el filtro o el orden
  onFilterOrOrderChange(): void {
    this.currentPage = 1;
    this.loadGradings();
  }

  // Paginación
  nextPage(): void {
    if (this.pagedResult && this.currentPage < this.pagedResult.totalPages) {
      this.currentPage++;
      this.loadGradings();
    }
  }

  prevPage(): void {
    if (this.pagedResult && this.currentPage > 1) {
      this.currentPage--;
      this.loadGradings();
    }
  }



}
