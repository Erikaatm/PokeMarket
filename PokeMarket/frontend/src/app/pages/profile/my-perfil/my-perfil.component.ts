import { Component, inject, Inject, PLATFORM_ID } from '@angular/core';
import { MenuComponent } from '../../../components/menu/menu.component';
import { FormsModule } from '@angular/forms';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { CreateCardComponent } from '../create-card/create-card.component';
import { MyCardsComponent } from '../my-cards/my-cards.component';
import { PersonalInfoComponent } from '../personal-info/personal-info.component';
import { RouterLink } from '@angular/router';
import { UserResponse } from '../../../models/DTOs/user-response';
import { AuthService } from '../../../services/auth/auth.service';
import { AdminComponent } from '../admin/admin.component';
import { TagPanelComponent } from '../tag-panel/tag-panel.component';
import { GradingComponent } from '../grading/grading.component';
import { RequestGradeComponent } from '../request-grade/request-grade.component';

@Component({
  selector: 'app-my-perfil',
  imports: [
    MenuComponent,
    FormsModule,
    CommonModule,
    MyCardsComponent,
    PersonalInfoComponent,
    CreateCardComponent,
    RouterLink,
    AdminComponent,
    TagPanelComponent,
    GradingComponent,
    RequestGradeComponent
  ],
  templateUrl: './my-perfil.component.html',
  styleUrl: './my-perfil.component.css',
})
export class MyPerfilComponent {
  user?: UserResponse;
  selectedTab = 'personal';

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
