import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Department } from 'src/app/models/Department'
import { CompanyService } from '../company/company.service';

@Injectable({
  providedIn: 'root'
})
export class CompanyDepartmentsService {
  private apiUrl = 'http://localhost:5106/api/department';

  constructor(private http: HttpClient, private companyService: CompanyService) { }

  getAll(): Observable<Department[]> {
    const id = this.companyService.getCompanyId();

    return this.http.get<Department[]>(`${this.apiUrl}/all?id=${id}`);
  }

  addDepartment(name: string, description: string) : Observable<any> {
    const companyId = this.companyService.getCompanyId();
    
    return this.http.post<any>(`${this.apiUrl}/create`, { name, description, companyId });
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
    return this.http.get<Department>(`${this.apiUrl}/get?id={id}`);
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
