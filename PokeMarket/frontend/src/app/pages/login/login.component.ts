import { Component } from '@angular/core';
import { AuthService } from '../../services/auth/auth.service';
import { Router } from '@angular/router';
import { LoginRequest } from '../../models/DTOs/login-request';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CartService } from '../../services/cart/cart.service';
import { MenuComponent } from '../../components/menu/menu.component';

@Component({
  selector: 'app-login',
  imports: [FormsModule, CommonModule, MenuComponent],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  // Declaración de variables
  email = '';
  password = '';
  errorMessage = '';

  constructor(
    private authService: AuthService,
    private cartService: CartService,
    private router: Router
  ) {}

  // Inicializamos las variables de email y password con el loginRequest
  onSubmit(): void {
    const loginRequest: LoginRequest = {
      email: this.email,
      password: this.password,
    };

    // Llamamos a la funcion del login que esta en el authService
    this.authService.login(loginRequest).subscribe({
      next: (response) => {
        // Guardamos el usuario que se ha logeado en el authService
        this.authService.setLoggedUser(response.user, response.token);

        // Fusionamos el carrito local con el del back
        this.cartService.mergeLocalWithBackCart();

        // Redirigimos a la ruta que hemos guardado o al home
        const redirectUrl = localStorage.getItem('redirectAfterLogin');
        if (redirectUrl) {
          localStorage.removeItem('redirectAfterLogin');
          this.router.navigate([redirectUrl]);
        } else {
          this.router.navigate(['/home']);
        }
      },
      error: (error) => {
        this.errorMessage = '';

        // Enviamos el mensaje de error
        if (error.error && error.error.message) {
          this.errorMessage = error.error.message;
        } else {
          this.errorMessage = 'Error al iniciar sesión.';
        }
      },
    });
  }

  // Limpia el error cuando empieza a escribir
  onInputChange(): void {
    this.errorMessage = '';
  }
}
