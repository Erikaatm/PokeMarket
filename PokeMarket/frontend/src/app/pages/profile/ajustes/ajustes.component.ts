import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { EditEmailComponent } from './edit-email/edit-email.component';
import { EditPasswordComponent } from './edit-password/edit-password.component';
import { EditProfileComponent } from './edit-profile/edit-profile.component';
import { UserResponse } from '../../../models/DTOs/user-response';
import { RouterLink } from '@angular/router';
import { MenuComponent } from '../../../components/menu/menu.component';
import { AuthService } from '../../../services/auth/auth.service';

@Component({
  selector: 'app-ajustes',
  imports: [
    FormsModule,
    CommonModule,
    EditEmailComponent,
    EditPasswordComponent,
    EditProfileComponent,
    RouterLink,
    MenuComponent,
  ],
  templateUrl: './ajustes.component.html',
  styleUrl: './ajustes.component.css',
})
export class AjustesComponent {
  user?: UserResponse;
  selectedTab = 'editarPerfil';

  constructor(
    private authService: AuthService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
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
