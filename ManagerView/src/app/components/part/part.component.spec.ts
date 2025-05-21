import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PartComponent } from './part.component';

describe('ProjectComponent', () => {
  let component: PartComponent;
  let fixture: ComponentFixture<PartComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PartComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
