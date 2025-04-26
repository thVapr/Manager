import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Employee } from 'src/app/models/Employee';
import { AuthService } from '../auth/auth.service';
import { PartLinksService } from '../part-links/part-links.service';
import { PartService } from '../part/part.service';

@Injectable({
  providedIn: 'root'
})
export class MemberService {
  private apiUrl = 'http://localhost:6732/api/members';

  constructor(private http: HttpClient, private authService : AuthService,
              private partLinksService : PartLinksService,
              private partService : PartService) { }

  getEmployeeById(id : string): Observable<Employee> {
    return this.http.get<Employee>(`${this.apiUrl}/get_member_profile?id=${id}`);
  }

  addEmployee(id : string, firstName? : string, lastName? : string, patronymic? : string) : Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/create`, {id, firstName, lastName, patronymic});
  }

  updateEmployee(id : string, firstName? : string, lastName? : string, patronymic? : string) : Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/update`, {id, firstName, lastName, patronymic});
  }

  isCurrentEmployee(employeeId : string | undefined) {
    const id = this.authService.getId();

    return employeeId === id;
  }

  getEmployeesByDepartmentId() : Observable<Employee[]> {
    const id = this.partLinksService.getDepartmentId();
    
    return this.http.get<Employee[]>(`${this.apiUrl}/all?id=${id}`);
  }

  getEmployeesByProjectId() : Observable<Employee[]> {
    const id = this.partService.getPartId();

    return this.http.get<Employee[]>(`${this.apiUrl}/get_part_members?id=${id}`);
  }

  getEmployeesWithoutDepartment() : Observable<Employee[]> {
    return this.http.get<Employee[]>(`${this.apiUrl}/get_free_members`);
  }

  getEmployeesWithoutProject() : Observable<Employee[]> {
    const id = this.partLinksService.getDepartmentId();

    return this.http.get<Employee[]>(`${this.apiUrl}/get_members?id=${id}`);
  }
}
