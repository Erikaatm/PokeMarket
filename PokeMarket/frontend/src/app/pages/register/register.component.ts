import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth/auth.service';
import { Router } from '@angular/router';
import { RegisterRequest } from '../../models/DTOs/register-request';
import { MenuComponent } from '../../components/menu/menu.component';

@Component({
  selector: 'app-register',
  imports: [FormsModule, CommonModule, MenuComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css',
})
export class RegisterComponent {
  // Declaración de variables
  username = '';
  email = '';
  confirmEmail = '';
  password = '';
  confirmPassword = '';

  errorMessage: string = '';
  successMessage: string = '';

  // Inicializamos en el constructor
  constructor(private authService: AuthService, private router: Router) {}

  // Inicializamos las variables
  onSubmit(): void {
    // OnSubmit porque queremos enviarlo nosotros con un boton
    const registerRequest: RegisterRequest = {
      username: this.username,
      emailAddress: this.email,
      confirmEmail: this.confirmEmail,
      password: this.password,
      confirmPassword: this.confirmPassword,
    };

    // Llamamos a la funcion de registro del authService
    this.authService.register(registerRequest).subscribe({
      next: (response) => {
        // Mostramos el mensaje de éxito del back
        this.successMessage = response.message;

        // Limpiamos el mensaje de error
        this.errorMessage = '';

        // Limpiamos el formulario
        this.username = '';
        this.email = '';
        this.confirmEmail = '';
        this.password = '';
        this.confirmPassword = '';
      },
      error: (error) => {
        // Esto lo uso para que me salgan ordenados los errores
        const errorOrder = [
          'Username',
          'EmailAddress',
          'ConfirmEmail',
          'Password',
          'ConfirmPassword',
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

  // Método para redirigir al login
  goToLogin(): void {
    this.router.navigate(['/login']);
  }

  // Método que limpia el error cuando el usuario empieza a escribir otra vez
  onInputChange(): void {
    this.errorMessage = '';
  }
}
