import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { MenuComponent } from '../../components/menu/menu.component';
import { isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'app-how-to-play',
  imports: [MenuComponent],
  templateUrl: './how-to-play.component.html',
  styleUrl: './how-to-play.component.css'
})
export class HowToPlayComponent {

  constructor(
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}


  ngOnInit() {
    if (isPlatformBrowser(this.platformId)) {
      document.body.className = 'fondo-perfil';
      
    }
  }
}
