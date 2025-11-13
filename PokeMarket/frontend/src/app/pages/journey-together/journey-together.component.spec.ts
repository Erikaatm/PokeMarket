import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JourneyTogetherComponent } from './journey-together.component';

describe('JourneyTogetherComponent', () => {
  let component: JourneyTogetherComponent;
  let fixture: ComponentFixture<JourneyTogetherComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [JourneyTogetherComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(JourneyTogetherComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
