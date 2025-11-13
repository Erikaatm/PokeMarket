import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CardResponse } from '../../../models/DTOs/card-response';
import { GradingService } from '../../../services/grading/grading.service';
import { PagedResult } from '../../../models/DTOs/pagedresult';

@Component({
  selector: 'app-request-grade',
  imports: [FormsModule, CommonModule],
  templateUrl: './request-grade.component.html',
  styleUrl: './request-grade.component.css',
})
export class RequestGradeComponent implements OnInit {
  // Declaramos
  cards: CardResponse[] = [];
  successMessage: string = '';
  errorMessage: string = '';

  // Variables para filtros
  pokemonType: string = '';
  collection: string = '';
  pokemonName: string = '';
  rarity: string = 'todos';
  gradedBy: string = '';
  selectedOrder: string = 'fecha-desc';

  // Paginación
  currentPage: number = 1;
  pagedResult: PagedResult<CardResponse> = {
    currentPage: 1,
    pageSize: 10,
    totalPages: 1,
    totalCount: 0,
    items: [],
  };

  constructor(private gradingService: GradingService) {}

  ngOnInit(): void {
    this.loadCardsAvailableForGrading();
  }

  // Método para cargar las cartas solicitadas a gradeo
  loadCardsAvailableForGrading(): void {
     this.gradingService.getMyUngradedCards().subscribe({
    next: (cards) => this.cards = cards,
    error: (err) => {
      this.cards = [];
      this.errorMessage = err.error?.message || 'Error al cargar las cartas.';
    }
  });
  }

  // Método para solicitar gradeo para una carta concreta
  requestGrading(cardId: number): void {
    this.gradingService.requestGrading(cardId).subscribe({
      next: (response) => {
        this.successMessage = response.message;
        this.errorMessage = '';
        // Recargar cartas disponibles
        this.loadCardsAvailableForGrading();
      },
      error: (error) => {
        this.successMessage = '';
        this.errorMessage =
          error.error?.message || 'Error al solicitar gradeo.';
      },
    });
  }

   onFilterOrOrderChange(): void {
    this.currentPage = 1;
    this.loadCardsAvailableForGrading();
  }

  // Paginación
  nextPage(): void {
    if (this.pagedResult && this.currentPage < this.pagedResult.totalPages) {
      this.currentPage++;
      this.loadCardsAvailableForGrading();
    }
  }

  prevPage(): void {
    if (this.pagedResult && this.currentPage > 1) {
      this.currentPage--;
      this.loadCardsAvailableForGrading();
    }
  }
}
