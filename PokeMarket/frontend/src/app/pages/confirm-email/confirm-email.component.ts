import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../services/auth/auth.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MenuComponent } from '../../components/menu/menu.component';

@Component({
  selector: 'app-confirm-email',
  standalone: true,
  imports: [FormsModule, CommonModule, MenuComponent],
  templateUrl: './confirm-email.component.html',
  styleUrl: './confirm-email.component.css'
})
export class ConfirmEmailComponent implements OnInit{
  // Declaración de variables
  successMessage: string = '';
  errorMessage: string = '';

  // Inicializamos lo que necesitemos en el constructor
  constructor(
    private route: ActivatedRoute,
    private authService: AuthService,
    private router: Router
  ) { }

  ngOnInit(): void {
      // Obtenemos el token de la URL 
      const token = this.route.snapshot.queryParamMap.get('token');

      // Si hay token 
      if (token) {
        // Llamamos al método de authService que hemos inicializado antes para confirmar el email
        this.authService.confirmEmail(token).subscribe({
          next: (response) => {
            // Si todo ha ido bien, podemos mostrar un mensaje de exito
            this.successMessage = response.message;
            this.errorMessage = '';

          },
          error: (error) => {
            console.log(error);
            this.errorMessage = "Error al verificar el email.";
            this.successMessage = '';
          }
        })
      } else {
        // Si no hay token, guardamos un error también
        this.errorMessage = "Token no válido.";
        this.successMessage = '';
      }
  }
}
