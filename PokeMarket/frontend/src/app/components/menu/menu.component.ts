import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth/auth.service';
import { FormsModule } from '@angular/forms';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { UserResponse } from '../../models/DTOs/user-response';

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [RouterLink, FormsModule, CommonModule],
  templateUrl: './menu.component.html',
  styleUrl: './menu.component.css',
})
export class MenuComponent {
  // Declaración de variables
  user: UserResponse | null = null;

  // Inicializamos en el constructor
  constructor(
    private router: Router,
    public authService: AuthService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  // Esto sirve para cargar automaticamente los elementos de la página
  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      this.authService.loadUserFromLocalStorage();
      // Nos suscribimos al BehaviorSubject
      this.authService.currentUser$.subscribe((user) => {
        this.user = user;
      });
    }
  }

  // Hacemos el método para redirigir segun pulse el usuario a una ruta distinra
  goToMenu(route: string): void {
    this.router.navigate([route]);
  }
}
