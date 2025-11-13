import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../services/auth/auth.service';
import { CardService } from '../../../services/card/card.service';
import { CreateCardRequest } from '../../../models/DTOs/create-card-request';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-create-card',
  imports: [FormsModule, CommonModule],
  templateUrl: './create-card.component.html',
  styleUrl: './create-card.component.css',
})
export class CreateCardComponent {
  // Declaración de los campos para crear una carta
  pokemonName = '';
  pokemonType = '';
  rarity = '';
  imageUrl = '';
  collection = '';
  graded = false;
  grade = 0;
  price!: number;
  isTradeable = false;
  tagIds: number[] = [];

  // Mensajes
  errorMessage = '';
  successMessage = '';

  // Para la imagen
  selectedFile!: File;
  previewUrl: string | null = null;

  constructor(
    private authService: AuthService,
    private cardService: CardService,
    private router: Router
  ) {}

  // Método para crear las cartas cuando le das a un botón
  onSubmit(): void {
    const request: CreateCardRequest = {
      pokemonName: this.pokemonName,
      pokemonType: this.pokemonType,
      rarity: this.rarity,
      imageUrl: this.imageUrl,
      collection: this.collection,
      graded: this.graded,
      grade: this.grade,
      price: this.price,
      is_tradeable: this.isTradeable,
      tagIds: this.tagIds,
    };

    // Llamamos al método del cardService
    this.cardService.createCard(request).subscribe({
      next: (response) => {
        this.successMessage = 'Carta creada correctamente.';
        this.errorMessage = '';
        this.resetForm();
      },
      error: (error) => {
        // Esto lo uso para que me salgan ordenados los errores
        const errorOrder = [
          'PokemonName',
          'PokemonType',
          'Rarity',
          'ImageUrl',
          'Collection',
          'Price',
          'Grade',
        ];

        // Guardamos los errores en una variable
        const errors = error.error.errors;

        if (errors) {
          // Ordenamos lso errores
          const orderedErrors = errorOrder
            .map((key) => errors[key])
            .filter((msgs) => msgs)
            .flat();

          // Los guardamos y mostramos el 1
          this.errorMessage =
            orderedErrors.length > 0
              ? orderedErrors[0]
              : 'Ha ocurrido un error inesperado.';
        } else if (error.error.message) {
          // Aqui cogemos el mensaje del back del controlador no del modelo
          this.errorMessage = error.error.message;
        } else {
          this.errorMessage = 'Ha ocurrido un error inesperado.';
        }
      },
    });
  }

  // Limpia errores al modificar campos
  onInputChange(): void {
    this.errorMessage = '';
  }

  // Método para limpiar el formulario
  resetForm(): void {
    this.pokemonName = '';
    this.pokemonType = '';
    this.rarity = '';
    this.imageUrl = '';
    this.collection = '';
    this.graded = false;
    this.grade = 0;
    this.price = 0;
    this.isTradeable = false;
    this.tagIds = [];
  }

  // Método para añadir imágenes ( preview)
  onFileSelected(event: Event): void {
    const fileInput = event.target as HTMLInputElement;

    if (fileInput.files && fileInput.files.length > 0) {
      this.selectedFile = fileInput.files[0];

      const reader = new FileReader();
      reader.onload = () => {
        this.previewUrl = reader.result as string;
      };
      reader.readAsDataURL(this.selectedFile);
    }
  }

  // Método para subir las imágenes al back
  uploadImg(): void {
    if (!this.selectedFile) {
      this.errorMessage = 'Por favor selecciona una imagen primero.';
      return;
    }

    // Llammos al método del servicio

    this.cardService.uploadImage(this.selectedFile).subscribe({
      next: (response) => {
        this.imageUrl = response.imageUrl; // Aquí ya tienes la URL para crear la carta
        this.successMessage = 'Imagen subida correctamente.';
        this.errorMessage = '';
      },
      error: () => {
        this.errorMessage = 'No se pudo subir la imagen.';
        this.successMessage = '';
      },
    });
  }
}
