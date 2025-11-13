import { HttpClient } from '@angular/common/http';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { LoginRequest } from '../../models/DTOs/login-request';
import { BehaviorSubject, Observable } from 'rxjs';
import { TokenRequest } from '../../models/DTOs/token-request';
import { ConfirmEmailResponse } from '../../models/DTOs/confirm-email-response';
import { RegisterRequest } from '../../models/DTOs/register-request';
import { ForgotPasswordRequest } from '../../models/DTOs/forgot-password-request';
import { ResetPasswordRequest } from '../../models/DTOs/reset-password-request';
import { ResendConfirmEmailRequest } from '../../models/DTOs/resend-confirm-email-request';
import { isPlatformBrowser } from '@angular/common';
import { environment } from '../../../environments/environment';
import { UserResponse } from '../../models/DTOs/user-response';
import { UpdateProfileRequest } from '../../models/DTOs/update-profile-request';
import { UpdateEmailRequest } from '../../models/DTOs/update-email-request';
import { UpdatePasswordRequest } from '../../models/DTOs/update-password-request';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  // Declaración de variables

  // Esto lo utilizo para guardar al usuario logeado
  currentUser: any = null;
  isLogged: boolean = false;

  // Esto lo utilizo para que se actualice el username en el menu
  // Creamos el Behavior con un valor inicial null
  private currentUserSubject = new BehaviorSubject<UserResponse | null>(null);

  // Y creamos un observable para que otros componentes puedan llamarlo
  currentUser$ = this.currentUserSubject.asObservable();

  // Url del metodo de auth
  private apiUrl = `${environment.apiUrl}/Auth`;

  // Inicializamos el httpClient
  constructor(
    private http: HttpClient,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  // Método para iniciar sesión
  login(loginRequest: LoginRequest): Observable<TokenRequest> {
    return this.http.post<TokenRequest>(`${this.apiUrl}/login`, loginRequest);
  }

  // Método para guardar al usuario logeado
  setLoggedUser(user: any, token: string) {
    this.currentUser = user;
    this.isLogged = true;

    // Lo guardamos en el localStorage
    localStorage.setItem('user', JSON.stringify(user));
    localStorage.setItem('token', token);

    // Actualizamos el BehaviorSubject
    this.currentUserSubject.next(user);
  }

  // Método para cerrar sesión
  logout() {
    this.currentUser = null;
    this.isLogged = false;
    localStorage.removeItem('user');
    localStorage.removeItem('token');
  }

  // Cuando carguemos la app, revisamos si hay algún usuario logeado
  loadUserFromLocalStorage() {
    if (isPlatformBrowser(this.platformId)) {
      const userLS = localStorage.getItem('user');
      const token = localStorage.getItem('token');

      // Comprobamos que el token de iniciar sesión ha expirado
      if (userLS && token) {
        if (this.isTokenExpired(token)) {
          this.logout();
          return;
        }

        const userParsed = JSON.parse(userLS);
        this.currentUser = userParsed;
        this.isLogged = true;
        this.currentUserSubject.next(userParsed);
      }

      if (userLS) {
      }
    }
  }

  // Método para saber si esta loggeado o no
  isLoggedIn(): boolean {
    const token = localStorage.getItem('token');
    return !!token;
  }

  setCurrentUser(user: UserResponse) {
    this.currentUserSubject.next(user);
  }

  getCurrentUserValue(): UserResponse | null {
    return this.currentUserSubject.value;
  }

  // Método para ver el perfil del usuario logeado
  getCurrentUserProfile(): Observable<UserResponse> {
    return this.http.get<UserResponse>(`${this.apiUrl}/me`);
  }

  confirmEmail(token: string) {
    // Guardamos la url que nos mandan en el back
    // Utilizamos encodeURIComponent, para que no haya caracteres especiales en el token
    const url = `${this.apiUrl}/confirm-email?token=${encodeURIComponent(
      token
    )}`;

    // Hacemos la petición GET para confirmar el email
    return this.http.get<ConfirmEmailResponse>(url);
  }

  // Método para registrarse
  register(registerRequest: RegisterRequest): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/register`, registerRequest);
  }

  // Método para enviar un correo de contraseña olvidada
  forgotPassword(
    forgotPasswordRequest: ForgotPasswordRequest
  ): Observable<any> {
    return this.http.post<any>(
      `${this.apiUrl}/forgot-password`,
      forgotPasswordRequest
    );
  }

  // Método para resetar la contraseña
  resetPassword(resetPasswordRequest: ResetPasswordRequest): Observable<any> {
    return this.http.post<any>(
      `${this.apiUrl}/reset-password`,
      resetPasswordRequest
    );
  }

  // Método para enviar otra vez el email de confirmación
  resendConfirmEmail(
    resendConfirmEmailRequest: ResendConfirmEmailRequest
  ): Observable<any> {
    return this.http.post<any>(
      `${this.apiUrl}/resend-confirm-email`,
      resendConfirmEmailRequest
    );
  }

  // Método para actualizar un perfil
  updateUserProfile(request: UpdateProfileRequest) {
    return this.http.put(`${this.apiUrl}/update-profile`, request);
  }
  updateUserEmail(request: UpdateEmailRequest) {
    return this.http.put(`${this.apiUrl}/update-email`, request);
  }
  updateUserPassword(request: UpdatePasswordRequest) {
    return this.http.put(`${this.apiUrl}/update-password`, request);
  }

  // Método para saber si el token de inicio de sesion ha expirado
  isTokenExpired(token: string): boolean {
    // Aquí cogemos la parte del medio (el payload), que contiene la fecha de expiración (exp)
    // atob decodifica el texto base64 a texto plano, y JSON.parse lo convierte a objeto
    const payload = JSON.parse(atob(token.split('.')[1]));

    // Obtenemos la fecha real en segundos
    const now = Math.floor(Date.now() / 1000);

    // Si la fecha de ahora es mayor a la del token este ha expirado
    return payload.exp < now;
  }
}
