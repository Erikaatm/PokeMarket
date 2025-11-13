import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Collection151Component } from './collection-151.component';

describe('Collection151Component', () => {
  let component: Collection151Component;
  let fixture: ComponentFixture<Collection151Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Collection151Component]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Collection151Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
