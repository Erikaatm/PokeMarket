import { Component } from '@angular/core';
import { AuthService } from '../../../../services/auth/auth.service';
import { UserService } from '../../../../services/user/user.service';
import { UpdateProfileRequest } from '../../../../models/DTOs/update-profile-request';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-edit-profile',
  imports: [FormsModule, CommonModule],
  templateUrl: './edit-profile.component.html',
  styleUrl: './edit-profile.component.css',
})
export class EditProfileComponent {
  // Declaración de campos de formulario
  username: string = '';
  firstName: string = '';
  lastName1: string = '';
  lastName2: string = '';
  phoneNumber: string = '';
  address: string = '';
  confirmEmail: string = '';
  confirmPassword: string = '';

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
    const cleanPhone = this.phoneNumber.trim();
    const request: UpdateProfileRequest = {
      userName: this.username,
      firstName: this.firstName,
      lastName1: this.lastName1,
      lastName2: this.lastName2,
      phoneNumber: cleanPhone === '' ? undefined : cleanPhone,
      address: this.address,
      currentEmail: this.confirmEmail,
      currentPassword: this.confirmPassword,
    };

    this.authService.updateUserProfile(request).subscribe({
      next: () => {
        this.successMessage = 'Perfil actualizado correctamente ✅';
        const currentUser = this.authService.getCurrentUserValue();
        if (currentUser) {
          this.authService.setCurrentUser({
            ...currentUser,
            username: this.username,
          });
        }
        this.username = '';
        this.firstName = '';
        this.lastName1 = '';
        this.lastName2 = '';
        this.phoneNumber = '';
        this.address = '';
        this.confirmEmail = '';
        this.confirmPassword = '';
      },
      error: (error) => {
        console.error('Error completo recibido:', error);

        const errors = error.error?.errors;

        if (errors) {
          const orderedErrors = Object.values(errors).flat() as string[];

          this.errorMessage =
            orderedErrors.length > 0
              ? orderedErrors[0]
              : 'Ha ocurrido un error inesperado.';
        } else if (error.error?.message) {
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
