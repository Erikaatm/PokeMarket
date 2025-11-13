import { Routes } from '@angular/router';
import { AllCardsComponent } from './pages/all-cards/all-cards.component';
import { HomeComponent } from './pages/home/home.component';
import { LoginComponent } from './pages/login/login.component';
import { ConfirmEmailComponent } from './pages/confirm-email/confirm-email.component';
import { RegisterComponent } from './pages/register/register.component';
import { ForgotPasswordComponent } from './pages/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './pages/reset-password/reset-password.component';
import { ResendConfirmEmailComponent } from './pages/resend-confirm-email/resend-confirm-email.component';
import { JourneyTogetherComponent } from './pages/journey-together/journey-together.component';
import { PrismaticEvolutionsComponent } from './pages/prismatic-evolutions/prismatic-evolutions.component';
import { SurgingSparksComponent } from './pages/surging-sparks/surging-sparks.component';
import { Collection151Component } from './pages/collection-151/collection-151.component';
import { ProfileComponent } from './pages/profile/profile.component';
import { CartComponent } from './pages/cart/cart.component';
import { authGuard } from './guards/auth.guard';
import { MyPerfilComponent } from './pages/profile/my-perfil/my-perfil.component';
import { FavoritesComponent } from './pages/favorites/favorites.component';
import { AjustesComponent } from './pages/profile/ajustes/ajustes.component';
import { EditProfileComponent } from './pages/profile/ajustes/edit-profile/edit-profile.component';
import { EditEmailComponent } from './pages/profile/ajustes/edit-email/edit-email.component';
import { EditPasswordComponent } from './pages/profile/ajustes/edit-password/edit-password.component';
import { GradingComponent } from './pages/profile/grading/grading.component';
import { HowToPlayComponent } from './pages/how-to-play/how-to-play.component';
import { PurchaseCompletedComponent } from './pages/purchase-completed/purchase-completed.component';

export const routes: Routes = [
    { path:'', redirectTo: '/home', pathMatch: 'full' },
    { path: 'allCards', component: AllCardsComponent },
    { path: 'home', component: HomeComponent },
    { path: 'login', component: LoginComponent },
    { path: 'confirm-email', component: ConfirmEmailComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'forgot-password', component: ForgotPasswordComponent },
    { path: 'reset-password', component: ResetPasswordComponent },
    { path: 'resend-confirm-email', component: ResendConfirmEmailComponent },
    { path: 'journey-together', component: JourneyTogetherComponent },
    { path: 'prismatic-evolutions', component: PrismaticEvolutionsComponent },
    { path: 'surging-sparks', component: SurgingSparksComponent },
    { path: '151', component: Collection151Component },
    { path: 'profile/:id', component: ProfileComponent, canActivate: [authGuard] },
    { path: 'ajustes/:id', component: AjustesComponent, canActivate: [authGuard] },
    { path: 'cart', component: CartComponent },
    { path: 'my-perfil', component: MyPerfilComponent },
    { path: 'favoritos', component: FavoritesComponent, canActivate: [authGuard] },
    { path: 'edit-profile', component: EditProfileComponent, canActivate: [authGuard] },
    { path: 'edit-email', component: EditEmailComponent, canActivate: [authGuard] },
    { path: 'edit-password', component: EditPasswordComponent, canActivate: [authGuard] },
    { path: 'grading-panel', component: GradingComponent, canActivate: [authGuard] },
    { path: 'how-to-play', component: HowToPlayComponent},
    { path: 'compra-realizada', component: PurchaseCompletedComponent},
];
