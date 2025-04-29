import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TaskProfileComponent } from './task-profile.component';

describe('TaskProfileComponent', () => {
  let component: TaskProfileComponent;
  let fixture: ComponentFixture<TaskProfileComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TaskProfileComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TaskProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
