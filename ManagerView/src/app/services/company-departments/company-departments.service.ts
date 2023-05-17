import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Department } from 'src/app/models/Department'
import { CompanyService } from '../company/company.service';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class CompanyDepartmentsService {
  private apiUrl = 'http://localhost:5106/api/department';

  constructor(private http: HttpClient,
     private companyService: CompanyService,
     private authService: AuthService) { }

  addManager(employeeId: string | undefined) : Observable<any> {
    const id = this.getDepartmentId();

    return this.updateDepartment(new Department(
      id!,
      "",
      "",
      employeeId!,
    ));

  }
 
  removeManager() : Observable<any> {
    const id = this.getDepartmentId();
    const emptyId = "00000000-0000-0000-0000-000000000000";

    return this.updateDepartment(new Department(
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

  getAll(): Observable<Department[]> {
    const id = this.companyService.getCompanyId();

    return this.http.get<Department[]>(`${this.apiUrl}/all?id=${id}`);
  }

  addDepartment(name: string, description: string) : Observable<any> {
    const companyId = this.companyService.getCompanyId();
    const managerId = this.authService.getId();
    
    return this.http.post<any>(`${this.apiUrl}/create`, { name, description, companyId, managerId });
  }
  
  addEmployeeToDepartment(employeeId : string) : Observable<any> {
    const departmentId = this.getDepartmentId();

    return this.http.post<any>(`${this.apiUrl}/add`,{ departmentId, employeeId });
  }

  removeEmployeeFromDepartment(employeeId : string) : Observable<any> {
    const departmentId = this.getDepartmentId();

    return this.http.post<any>(`${this.apiUrl}/remove`,{ departmentId, employeeId });
  }

  updateDepartment(department : Department) : Observable<any> {
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

  getDepartment(id : string) : Observable<Department> {
    return this.http.get<Department>(`${this.apiUrl}/get?id=${id}`);
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
