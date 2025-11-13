import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TagPanelComponent } from './tag-panel.component';

describe('TagPanelComponent', () => {
  let component: TagPanelComponent;
  let fixture: ComponentFixture<TagPanelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TagPanelComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TagPanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
