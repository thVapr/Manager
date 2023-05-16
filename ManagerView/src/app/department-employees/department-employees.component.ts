import { Component } from '@angular/core';
import { CompanyDepartmentsService } from '../services/company-departments/company-departments.service';
import { EmployeeService } from '../services/employee/employee.service';
import { Employee } from '../models/Employee';
import { AuthService } from '../services/auth/auth.service';
import { Department } from '../models/Department';

@Component({
  selector: 'app-department-employees',
  templateUrl: './department-employees.component.html',
  styleUrls: ['./department-employees.component.scss']
})
export class DepartmentEmployeesComponent {
  employees : Employee[] = [];
  departmentEmployees: Employee[] = [];
  department : Department = new Department("","","","");

  constructor(public departmentService: CompanyDepartmentsService,
              public employeeService: EmployeeService,
              public authService: AuthService) {}

  ngOnInit(): void {
    this.Update();
  }

  addManager(employeeId: string | undefined) {
    this.departmentService.addManager(employeeId).subscribe(() => {
      this.Update();
    });
  }  

  removeManager() {
    this.departmentService.removeManager().subscribe(() => {
      this.Update();
    });
  }

  Update() : void {
    this.GetAll();
    this.GetAllFree();

    const id = this.departmentService.getDepartmentId();

    if(id !== null)
      this.departmentService.getDepartment(id).subscribe((department) => {
        this.department = department;
      });
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
