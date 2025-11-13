import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TagResponse } from '../../../models/DTOs/tag-response';
import { TagService } from '../../../services/tag/tag.service';
import { CardResponse } from '../../../models/DTOs/card-response';
import { PagedResult } from '../../../models/DTOs/pagedresult';

@Component({
  selector: 'app-tag-panel',
  imports: [FormsModule, CommonModule],
  templateUrl: './tag-panel.component.html',
  styleUrl: './tag-panel.component.css',
})
export class TagPanelComponent {
  // Declaracion
  tags: TagResponse[] = [];
  tagName: string = '';
  searchTag: string = '';
  orderBy: string = 'fexha-desc';
  currentPage: number = 1;
  errorMessage = '';
  successMessage = '';

  // Paginado para las cartas
  pagedResult: PagedResult<TagResponse> = {
    currentPage: 1,
    pageSize: 10,
    totalPages: 1,
    totalCount: 0,
    items: [],
  };

  // Constructor
  constructor(private tagService: TagService) {}

  // Método para que cargue directamente
  ngOnInit(): void {
    this.loadTags();
  }

  // Método para cargar cartas
  loadTags(): void {
    this.tagService
      .getAllTags(this.currentPage, this.searchTag, this.orderBy)
      .subscribe({
        next: (response) => {
          (this.pagedResult = response),
            (this.tags = response.items),
            (this.errorMessage = '');
        },
        error: (error) => {
          this.errorMessage =
            error.error.message || 'Error al cargar los tags.';
          this.tags = [];
        },
      });
  }

  // Método para crear tags
  createTag(): void {
    // Comprobamos que hay algo en tagName
    if (!this.tagName.trim()) return;

    // Convertimos para que no haya ningun espacio
    const tag = { name: this.tagName.trim() };

    // Si lo hay llamamos al método del servicio
    this.tagService.createTag(tag).subscribe({
      next: () => {
        this.tagName = '';
        this.loadTags(); // Recarga la lista
      },
      error: (error) => {
        this.errorMessage = error.error.message || 'Error al crear el tag.';
      },
    });
  }

  // Método para eiliminar tags
  deleteTag(tagId: number): void {
    if (!confirm('¿Seguro que quieres eliminar este tag?')) return;

    this.tagService.deleteTag(tagId).subscribe({
      next: () => {
        this.loadTags(); // Refresca
      },
      error: (error) => {
        this.errorMessage = error.error.message || 'Error al eliminar el tag.';
      },
    });
  }

  // Método para los filtros y paginado
  onFilterChange(): void {
    this.currentPage = 1;
    this.loadTags();
  }

  nextPage(): void {
    if (this.currentPage < this.pagedResult.totalPages) {
      this.currentPage++;
      this.loadTags();
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadTags();
    }
  }
}
