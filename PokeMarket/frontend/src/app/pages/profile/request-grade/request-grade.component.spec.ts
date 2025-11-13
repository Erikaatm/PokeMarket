import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RequestGradeComponent } from './request-grade.component';

describe('RequestGradeComponent', () => {
  let component: RequestGradeComponent;
  let fixture: ComponentFixture<RequestGradeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RequestGradeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RequestGradeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
