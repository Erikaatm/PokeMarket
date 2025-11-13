import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MenuComponent } from '../../components/menu/menu.component';
import { isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'app-home',
  imports: [RouterLink, MenuComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {

  constructor( @Inject(PLATFORM_ID) private platformId: Object) { }

   ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      document.body.className = 'fondo-home';
    }
  }
}
