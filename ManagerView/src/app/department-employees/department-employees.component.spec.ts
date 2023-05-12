import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DepartmentEmployeesComponent } from './department-employees.component';

describe('DepartmentEmployeesComponent', () => {
  let component: DepartmentEmployeesComponent;
  let fixture: ComponentFixture<DepartmentEmployeesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DepartmentEmployeesComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DepartmentEmployeesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
