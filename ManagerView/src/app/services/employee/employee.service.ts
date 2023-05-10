import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map, catchError, of } from 'rxjs';
import { Employee } from 'src/app/models/Employee';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  private apiUrl = 'http://localhost:5106/api/employee';

  constructor(private http: HttpClient, private authService : AuthService) { }

  getAll(): Observable<Employee[]> {
    return this.http.get<Employee[]>(`${this.apiUrl}/all`);
  }

  getEmployeeById(id : string): Observable<Employee> {
    return this.http.get<Employee>(`${this.apiUrl}/get?id=${id}`);
  }

  addEmployee(userId : string, firstName? : string, lastName? : string, patronymic? : string) : Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/create`, {userId, firstName, lastName, patronymic});
  }

}
