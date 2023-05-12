import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Project } from 'src/app/models/Project';
import { CompanyDepartmentsService } from '../company-departments/company-departments.service';

@Injectable({
  providedIn: 'root'
})
export class ProjectService {
  private apiUrl = 'http://localhost:5106/api/project';

  constructor(private http: HttpClient, private departmentService : CompanyDepartmentsService) { }

  getAll(): Observable<Project[]> {
    const id = this.departmentService.getDepartmentId();

    return this.http.get<Project[]>(`${this.apiUrl}/all?id=${id}`);
  }

  getProjectById(id : string): Observable<Project> {
    return this.http.get<Project>(`${this.apiUrl}/get?id=${id}`);
  }

  addProject(name : string, description : string ) : Observable<any> {
    const departmentId = this.departmentService.getDepartmentId();
    
    return this.http.post<any>(`${this.apiUrl}/create`, { departmentId, name, description });
  }

  addEmployeeToProject(employeeId : string) : Observable<any> {
    const projectId = this.getProjectId();

    return this.http.post<any>(`${this.apiUrl}/add`,{ projectId, employeeId });
  }
  
  removeEmployeeFromProject(employeeId : string) : Observable<any> {
    const projectId = this.getProjectId();

    return this.http.post<any>(`${this.apiUrl}/remove`,{ projectId, employeeId });
  }
  
  isProjectSelected() : boolean {
    const id = this.getProjectId();
    const name = this.getProjectName();

    return id !== null && name !== null;
  }

  setProjectId(id : string) : void {
    localStorage.setItem('project_id',id);
  }

  getProjectId() : string | null {
    const id = localStorage.getItem('project_id');
    
    return id;
  }

  setProjectName(name : string) : void {
    localStorage.setItem('project_name', name);
  }

  removeProjectData() : void {
    localStorage.removeItem('project_id');
    localStorage.removeItem('project_name');
  }

  getProjectName() : string | null {
    const name = localStorage.getItem('project_name');

    return name;
  }
}
