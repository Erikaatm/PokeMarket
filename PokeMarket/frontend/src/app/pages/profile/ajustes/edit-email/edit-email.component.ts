import { Component } from '@angular/core';
import { UpdateEmailRequest } from '../../../../models/DTOs/update-email-request';
import { AuthService } from '../../../../services/auth/auth.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-edit-email',
  imports: [FormsModule, CommonModule],
  templateUrl: './edit-email.component.html',
  styleUrl: './edit-email.component.css',
})
export class EditEmailComponent {
  // Declaración de campos de formulario
  currentEmail: string = '';
  newEmail: string = '';
  confirmNewEmail: string = '';
  currentPassword: string = '';

  // Mensajes
  errorMessage: string = '';
  successMessage: string = '';

  constructor(private authService: AuthService) {}

  // Método para que cargue automaticamente
  onSubmit(): void {
    this.updatePerfil();
  }

  // Método para actualizar el perfil
  updatePerfil(): void {
    const request: UpdateEmailRequest = {
      currentEmail: this.currentEmail,
      newEmail: this.newEmail,
      confirmNewEmail: this.confirmNewEmail,
      currentPassword: this.currentPassword,
    };

    this.authService.updateUserEmail(request).subscribe({
      next: () => {
        this.successMessage = 'Perfil actualizado correctamente ✅';

        this.currentEmail = '';
        this.newEmail = '';
        this.confirmNewEmail = '';
        this.currentPassword = '';
      },
      error: (error) => {
        console.error('Error recibido:', error);

        // Esto lo uso para que me salgan ordenados los errores
        const errorOrder = [
          'CurrentEmail',
          'NewEmail',
          'ConfirmNewEmail',
          'CurrentPassword',
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

        this.successMessage = '';
      },
    });
  }

  // Método que limpia el error cuando el usuario empieza a escribir otra vez
  onInputChange(): void {
    this.errorMessage = '';
  }
}
