import { Component } from '@angular/core';
import { UserResponse } from '../../../models/DTOs/user-response';
import { UserService } from '../../../services/user/user.service';
import { response } from 'express';
import { PagedResult } from '../../../models/DTOs/pagedresult';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-admin',
  imports: [FormsModule, CommonModule],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.css',
})
export class AdminComponent {
  // Declaramos
  users: UserResponse[] = [];

  // Paginado para las cartas
  pagedResult: PagedResult<UserResponse> = {
    currentPage: 1,
    pageSize: 10,
    totalPages: 1,
    totalCount: 0,
    items: [],
  };

  // Filtros
  username: string = '';
  email: string = '';
  role: string = '';

  // ordenacion
  orderBy: string = 'fecha-desc';
  currentPage: number = 1;
  errorMessage: string = '';

  constructor(private userService: UserService) {}

  // Método para cargar los usuarios nada más cargar la página
  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    // Llamamos al método en el servicio
    this.userService.getUsers(
      this.currentPage,
      this.username,
      this.email,
      this.role,
      this.orderBy
    ).subscribe({
      next: (response) => {
        this.pagedResult = response,
        this.users = response.items,
        this.errorMessage = ''
      },
      error: (error) => {
        this.users = [],
        this.errorMessage = error.error?.message || 'Error al cargar los usuarios.';
      }

    });
  }

  onFilterOrOrderChange(): void {
    this.currentPage = 1;
    this.loadUsers();
  }

  nextPage(): void {
    if (this.currentPage < this.pagedResult.totalPages) {
      this.currentPage++;
      this.loadUsers();
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadUsers();
    }
  }

  // Método para eliminar a un usuario
  deleteUser(userId: number): void {
    if (confirm('¿Estás seguro de eilimar a este usuario?')) {
      this.userService.deleteUser(userId).subscribe({
        next: (response) => {
          this.loadUsers();
        },
        error: (error) => {
          this.errorMessage = error.error?.message || 'Error al eliminar el usuario.';
        }
      });
    }
  }
}
