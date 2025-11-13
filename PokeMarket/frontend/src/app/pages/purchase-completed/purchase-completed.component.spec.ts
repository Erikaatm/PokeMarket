import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PurchaseCompletedComponent } from './purchase-completed.component';

describe('PurchaseCompletedComponent', () => {
  let component: PurchaseCompletedComponent;
  let fixture: ComponentFixture<PurchaseCompletedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PurchaseCompletedComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PurchaseCompletedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
