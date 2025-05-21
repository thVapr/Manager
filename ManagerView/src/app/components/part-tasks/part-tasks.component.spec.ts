import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PartTasksComponent } from './part-tasks.component';

describe('PartTasksComponent', () => {
  let component: PartTasksComponent;
  let fixture: ComponentFixture<PartTasksComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PartTasksComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PartTasksComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
