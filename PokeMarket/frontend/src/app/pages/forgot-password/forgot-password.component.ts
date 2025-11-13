import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth/auth.service';
import { Router } from '@angular/router';
import { ForgotPasswordRequest } from '../../models/DTOs/forgot-password-request';
import { MenuComponent } from '../../components/menu/menu.component';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [FormsModule, CommonModule, MenuComponent],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.css',
})
export class ForgotPasswordComponent {
  email = '';
  successMessage = '';
  errorMessage = '';

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit(): void {
    const forgotPasswordResquest: ForgotPasswordRequest = {
      email: this.email,
    };

    this.authService.forgotPassword(forgotPasswordResquest).subscribe({
      next: (response) => {
        this.successMessage = response.message;
        this.errorMessage = '';
        this.email = ''; // Limpiamos el email
      },
      error: (error) => {
        this.errorMessage = error.error.errors;
        this.successMessage = '';
      },
    });
  }
}
