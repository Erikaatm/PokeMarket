import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PrismaticEvolutionsComponent } from './prismatic-evolutions.component';

describe('PrismaticEvolutionsComponent', () => {
  let component: PrismaticEvolutionsComponent;
  let fixture: ComponentFixture<PrismaticEvolutionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PrismaticEvolutionsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PrismaticEvolutionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
