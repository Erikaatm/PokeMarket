import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth/auth.service';
import { Router } from '@angular/router';
import { ResendConfirmEmailRequest } from '../../models/DTOs/resend-confirm-email-request';
import { MenuComponent } from '../../components/menu/menu.component';

@Component({
  selector: 'app-resend-confirm-email',
  standalone: true,
  imports: [FormsModule, CommonModule, MenuComponent],
  templateUrl: './resend-confirm-email.component.html',
  styleUrl: './resend-confirm-email.component.css'
})
export class ResendConfirmEmailComponent {
  // Declaración de variables
  email = '';
  successMessage = '';
  errorMessage = '';

  // Inicializamos en el constructor
  constructor(private authService: AuthService, private router: Router) { }

  // Llamamos al método del authService para reenviar el email de confirmacion 
  onSubmit(): void {
    // Inicializamos las variables
    const resendConfirmEmailRequest: ResendConfirmEmailRequest = {
      email: this.email
    }

    // Llamamos al servicio 
    this.authService.resendConfirmEmail(resendConfirmEmailRequest).subscribe({
      next: (response) => {
        // Mostramos el mensaje de éxito
        this.successMessage = response.message;
        this.errorMessage = '';

        // Limpiamos el campo de email
        this.email = '';
      },
      error: (error) => {
        this.errorMessage = '';

        // Tenemos que comprobar que tipo de error es
        if (error.error.message) {
          this.errorMessage = error.error.message;

        } else if (error.error && error.error.errors) {
          this.errorMessage = error.error.errors;
          
        } else {
          this.errorMessage = 'Error al iniciar sesión.';
        }
      }
    });
  }

}
