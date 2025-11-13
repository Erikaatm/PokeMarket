import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { AuthService } from '../../services/auth/auth.service';
import { Router, RouterLink } from '@angular/router';
import { MenuComponent } from '../../components/menu/menu.component';
import { isPlatformBrowser } from '@angular/common';
import { UserResponse } from '../../models/DTOs/user-response';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [MenuComponent, RouterLink],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css',
})
export class ProfileComponent {
  user?: UserResponse;

  constructor(
    private authService: AuthService,
    private router: Router,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  // Método de cerrar sesión
  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  ngOnInit() {
    if (isPlatformBrowser(this.platformId)) {
      document.body.className = 'fondo-perfil';
      this.authService.getCurrentUserProfile().subscribe({
        next: (data) => {
          this.user = data;
        },
        error: () => {
          this.user = undefined;
        },
      });
    }
  }
}
