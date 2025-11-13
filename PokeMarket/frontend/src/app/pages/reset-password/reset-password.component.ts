import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ResetPasswordRequest } from '../../models/DTOs/reset-password-request';
import { MenuComponent } from '../../components/menu/menu.component';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [FormsModule, CommonModule, MenuComponent],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css',
})
export class ResetPasswordComponent {
  // Declaramos variables
  token = '';
  newPassword = '';
  confirmNewPassword = '';
  successMessage = '';
  errorMessage = '';

  // Inicializamos el constructor
  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  // Guardamos el token que viene de la URL
  ngOnInit(): void {
    this.token = this.route.snapshot.queryParamMap.get('token') || '';
  }

  // Hacemos el método para llamar al servicio de reset password cuando le demos al boton
  onSubmit(): void {
    const resetPasswordRequest: ResetPasswordRequest = {
      token: this.token,
      newPassword: this.newPassword,
      confirmNewPassword: this.confirmNewPassword,
    };

    // Llamamos al metodo de resetPassword
    this.authService.resetPassword(resetPasswordRequest).subscribe({
      next: (response) => {
        // Mostramos el mensaje de éxito
        this.successMessage = response.message;
        this.errorMessage = '';

        //Limpiamos los campos
        this.newPassword = '';
        this.confirmNewPassword = '';
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
      },
    });
  }
}
