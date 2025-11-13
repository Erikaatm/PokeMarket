import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GradedComponent } from './graded.component';

describe('GradedComponent', () => {
  let component: GradedComponent;
  let fixture: ComponentFixture<GradedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GradedComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GradedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
