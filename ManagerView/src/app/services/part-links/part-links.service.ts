import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Part } from 'src/app/models/Part'
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class PartLinksService {
  private apiUrl = 'http://localhost:6732/api/parts';

  constructor(private http: HttpClient,
     private authService: AuthService) { }

  addManager(employeeId: string | undefined) : Observable<any> {
    const id = this.getDepartmentId();

    return this.updateDepartment(new Part(
      id!,
      "",
      "",
      employeeId!,
    ));

  }
 
  removeManager() : Observable<any> {
    const id = this.getDepartmentId();
    const emptyId = "00000000-0000-0000-0000-000000000000";

    return this.updateDepartment(new Part(
      id!,
      "",
      "",
      emptyId,
    ));
  }

  isManagerExist(managerId : string | undefined): boolean {

    if (managerId !== null &&
        managerId !== "" &&
        managerId !== "00000000-0000-0000-0000-000000000000" &&
        managerId !== undefined)
      return true;
    return false;
  }

  isManager(id : string | undefined, managerId : string | undefined) : boolean {
    if (managerId === id)
      return true;
    return false;    
  }

  getAll(): Observable<Part[]> {

    return new Observable<Part[]>();
  }

  addDepartment(name: string, description: string) : Observable<any> {
    const managerId = this.authService.getId();
    
    return this.http.post<any>(`${this.apiUrl}/create`, { name, description, managerId });
  }
  
  addEmployeeToDepartment(employeeId : string) : Observable<any> {
    const departmentId = this.getDepartmentId();

    return this.http.post<any>(`${this.apiUrl}/add`,{ departmentId, employeeId });
  }

  removeEmployeeFromDepartment(employeeId : string) : Observable<any> {
    const departmentId = this.getDepartmentId();

    return this.http.post<any>(`${this.apiUrl}/remove`,{ departmentId, employeeId });
  }

  updateDepartment(department : Part) : Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/update`, department);
  }

  setDepartmentId(id : string) : void {
    localStorage.setItem('department_id',id);
  }

  setDepartmentName(name : string) : void {
    localStorage.setItem('department_name', name);
  }

  getDepartmentName() : string | null {
    const name = localStorage.getItem('department_name');
    return name;
  }

  getDepartmentId() : string | null {
    const id = localStorage.getItem('department_id');
    return id;
  }

  getPart(id : string) : Observable<Part> {
    return this.http.get<Part>(`${this.apiUrl}/get?id=${id}`);
  }

  removeDepatmentData() : void {
    localStorage.removeItem('department_id');
    localStorage.removeItem('department_name');
  }
  
  isDepartmentSelected() : boolean {
    const id = this.getDepartmentId();
    const name = this.getDepartmentName();

    return id !== null && name !== null;
  }
}
