import { Component, Inject, OnInit, PLATFORM_ID } from '@angular/core';
import { GradingResponse } from '../../../models/DTOs/grading-response';
import { PagedResult } from '../../../models/DTOs/pagedresult';
import { GradingService } from '../../../services/grading/grading.service';
import { GradeCardRequest } from '../../../models/DTOs/grade-card-request';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CardResponse } from '../../../models/DTOs/card-response';
import { GradedComponent } from './graded/graded.component';
import { RequestedComponent } from './requested/requested.component';
import { UserResponse } from '../../../models/DTOs/user-response';
import { AuthService } from '../../../services/auth/auth.service';

@Component({
  selector: 'app-grading',
  imports: [FormsModule, CommonModule, GradedComponent, RequestedComponent],
  templateUrl: './grading.component.html',
  styleUrl: './grading.component.css',
})
export class GradingComponent implements OnInit {
  user?: UserResponse;
  selectedTab = 'requested';

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
