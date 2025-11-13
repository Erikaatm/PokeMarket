import { inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { CanActivateFn } from '@angular/router';

export const authGuard: CanActivateFn = () => {
  const platformId = inject(PLATFORM_ID);

  // Si estamos en el navegador, comprobamos el token
  if (isPlatformBrowser(platformId)) {
    const token = localStorage.getItem('token');
    return !!token; // true si existe, false si no
  }

  // Si estamos en el servidor (SSR), dejamos pasar por ahora
  // para evitar errores y permitir la navegación inicial
  return true;
};
