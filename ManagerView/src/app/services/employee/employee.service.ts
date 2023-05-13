import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Employee } from 'src/app/models/Employee';
import { AuthService } from '../auth/auth.service';
import { CompanyDepartmentsService } from '../company-departments/company-departments.service';
import { ProjectService } from '../project/project.service';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  private apiUrl = 'http://localhost:5106/api/employee';

  constructor(private http: HttpClient, private authService : AuthService,
              private departmentService : CompanyDepartmentsService,
              private projectService : ProjectService) { }

  getEmployeeById(id : string): Observable<Employee> {
    return this.http.get<Employee>(`${this.apiUrl}/get_employee_profile?id=${id}`);
  }

  addEmployee(id : string, firstName? : string, lastName? : string, patronymic? : string) : Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/create`, {id, firstName, lastName, patronymic});
  }

  getEmployeesByDepartmentId() : Observable<Employee[]> {
    const id = this.departmentService.getDepartmentId();
    
    return this.http.get<Employee[]>(`${this.apiUrl}/all?id=${id}`);
  }

  getEmployeesByProjectId() : Observable<Employee[]> {
    const id = this.projectService.getProjectId();

    return this.http.get<Employee[]>(`${this.apiUrl}/get_project_employees?id=${id}`);
  }

  getEmployeesWithoutDepartment() : Observable<Employee[]> {
    return this.http.get<Employee[]>(`${this.apiUrl}/get_free_employees`);
  }

  getEmployeesWithoutProject() : Observable<Employee[]> {
    const id = this.departmentService.getDepartmentId();

    return this.http.get<Employee[]>(`${this.apiUrl}/get_employees?id=${id}`);
  }

}
