import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Project } from 'src/app/models/Project';
import { PartLinksService } from '../part-links/part-links.service';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class PartService {
  private apiUrl = 'http://localhost:6732/api/parts';

  constructor(private http: HttpClient,
     private partLinksService : PartLinksService,
     private authService: AuthService) { }

  addManager(employeeId: string | undefined) : Observable<any> {
    const id = this.getPartId();

    return this.updateProject(new Project(
      id!,
      "",
      "",
      employeeId!,
    ));
  }

  isManagerExist(managerId : string | undefined): boolean {
    console.log(managerId);
    if (managerId !== null &&
        managerId !== "" &&
        managerId !== "00000000-0000-0000-0000-000000000000" &&
        managerId !== undefined)
      return true;
    return false;
  }
  
  isManager(id : string | undefined, managerId : string | undefined) : boolean {
    if (this.isManagerExist(managerId) && managerId == id) {
      return true;
    }
    return false;    
  }

  removeManager() : Observable<any> {
    const id = this.getPartId();
    const emptyId = "00000000-0000-0000-0000-000000000000";

    return this.updateProject(new Project(
      id!,
      "",
      "",
      emptyId,
    ));
  }

  getAll(): Observable<Project[]> {
    const id = this.partLinksService.getDepartmentId();

    return this.http.get<Project[]>(`${this.apiUrl}/all?id=${id}`);
  }

  getProjectById(id : string): Observable<Project> {
    return this.http.get<Project>(`${this.apiUrl}/get?id=${id}`);
  }

  addProject(name : string, description : string ) : Observable<any> {
    const departmentId = this.partLinksService.getDepartmentId();
    const managerId = this.authService.getId();

    return this.http.post<any>(`${this.apiUrl}/create`, { departmentId, name, description, managerId });
  }

  updateProject(project: Project) : Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/update`, project);
  }

  addEmployeeToProject(employeeId : string) : Observable<any> {
    const projectId = this.getPartId();

    return this.http.post<any>(`${this.apiUrl}/add`,{ projectId, employeeId });
  }
  
  removeEmployeeFromProject(employeeId : string) : Observable<any> {
    const projectId = this.getPartId();

    return this.http.post<any>(`${this.apiUrl}/remove`,{ projectId, employeeId });
  }
  
  isPartSelected() : boolean {
    const id = this.getPartId();
    const name = this.getPartName();

    return id !== null && name !== null;
  }

  setPartId(id : string) : void {
    localStorage.setItem('project_id',id);
  }

  getPartId() : string | null {
    const id = localStorage.getItem('project_id');
    
    return id;
  }

  setPartName(name : string) : void {
    localStorage.setItem('project_name', name);
  }

  removePartData() : void {
    localStorage.removeItem('project_id');
    localStorage.removeItem('project_name');
  }

  getPartName() : string | null {
    const name = localStorage.getItem('project_name');

    return name;
  }
}
