import { Component } from '@angular/core';
import { CompanyDepartmentsService } from '../services/company-departments/company-departments.service';
import { EmployeeService } from '../services/employee/employee.service';
import { Employee } from '../models/Employee';

@Component({
  selector: 'app-department-employees',
  templateUrl: './department-employees.component.html',
  styleUrls: ['./department-employees.component.scss']
})
export class DepartmentEmployeesComponent {
  employees : Employee[] = [];
  departmentEmployees: Employee[] = [];

  constructor(public departmentService: CompanyDepartmentsService,
              public employeeService: EmployeeService) {}

  ngOnInit(): void {
    this.Update();
  }

  OnChoose(id : string | undefined) : void {

  }

  Update() : void {
    this.GetAll();
    this.GetAllFree();
  }

  AddEmployeeToDepartment(id : any) {
    this.departmentService.addEmployeeToDepartment(id).subscribe(() => this.Update());
  }

  RemoveEmployeeFromDepartment(id : any) {
    this.departmentService.removeEmployeeFromDepartment(id).subscribe(() => this.Update());
  }

  GetAllFree() : void {
    this.employeeService.getEmployeesWithoutDepartment().subscribe(employees => this.employees = employees);
  }

  GetAll() : void {
    this.employeeService.getEmployeesByDepartmentId().subscribe(employees => this.departmentEmployees = employees);
  }
}
