import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { UserResponse } from '../../../models/DTOs/user-response';
import { UserService } from '../../../services/user/user.service';
import { AuthService } from '../../../services/auth/auth.service';

@Component({
  selector: 'app-personal-info',
  imports: [FormsModule, CommonModule],
  templateUrl: './personal-info.component.html',
  styleUrl: './personal-info.component.css',
})
export class PersonalInfoComponent {
  // Declaración de variables
  user?: UserResponse; // Guardamos los datos del usuario
  errorMessage = '';

  // Constructor para llamar a los servicios
  constructor(
    private authService: AuthService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  // Cuando cargue la página nos muestra la info
  ngOnInit() {
    if (isPlatformBrowser(this.platformId)) {
      document.body.className = 'fondo-perfil';
      this.authService.getCurrentUserProfile().subscribe({
        next: (data) => {
          this.user = data;
        },
        error: (error) => {
          if (error.error && error.error.message) {
            this.errorMessage = error.error.message;
          } else {
            this.errorMessage = 'Se ha producido un error.';
          }
        },
      });
    }
  }
}
