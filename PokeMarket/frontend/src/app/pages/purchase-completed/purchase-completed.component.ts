import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { UserResponse } from '../../models/DTOs/user-response';
import { CartItemResponse } from '../../models/DTOs/cart-item-response';
import { AuthService } from '../../services/auth/auth.service';
import { CartService } from '../../services/cart/cart.service';
import { FormsModule } from '@angular/forms';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { MenuComponent } from '../../components/menu/menu.component';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-purchase-completed',
  standalone: true,
  imports: [FormsModule, CommonModule, MenuComponent, RouterLink],
  templateUrl: './purchase-completed.component.html',
  styleUrl: './purchase-completed.component.css',
})
export class PurchaseCompletedComponent {
  user: UserResponse | null = null;
  cartItems: CartItemResponse[] = [];
  errorMessage: string = '';

  constructor(
    private authService: AuthService,
    private cartService: CartService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

  ngOnInit(): void {
    if (!isPlatformBrowser(this.platformId)) return;

    document.body.className = 'cart-body';

    // Obtener perfil del usuario
    this.authService.getCurrentUserProfile().subscribe({
      next: (res) => {
        this.user = res;
      },
      error: () => {
        this.errorMessage = 'No se pudo obtener el perfil del usuario.';
      },
    });

    // Obtener contenido del carrito
    this.cartService.getCart().subscribe({
      next: (res) => {
        this.cartItems = res.carrito;

        if (this.cartItems.length === 0) {
          this.errorMessage = 'Compra realizada.';
          return;
        }

        // Vaciar carrito después de mostrar las cartas
        const result = this.cartService.clearCart();
        if (result && result.subscribe) {
          result.subscribe();
        }
        this.errorMessage = '';
      },
      error: () => {
        this.errorMessage = 'No se pudo obtener el contenido del carrito.';
      },
    });
  }
}
