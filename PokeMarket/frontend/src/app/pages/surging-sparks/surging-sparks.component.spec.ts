import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SurgingSparksComponent } from './surging-sparks.component';

describe('SurgingSparksComponent', () => {
  let component: SurgingSparksComponent;
  let fixture: ComponentFixture<SurgingSparksComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SurgingSparksComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SurgingSparksComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
