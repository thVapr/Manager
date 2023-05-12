import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ProjectService } from '../project/project.service';
import { HttpClient } from '@angular/common/http';
import { Task } from 'src/app/models/Task';
import { CompanyDepartmentsService } from '../company-departments/company-departments.service';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private apiUrl = 'http://localhost:5106/api/task';

  constructor(private http: HttpClient,
              private projectService : ProjectService,
              private departmentService: CompanyDepartmentsService,
              private authService: AuthService) { }


  getAll(): Observable<Task[]> {
    const id = this.projectService.getProjectId();

    return this.http.get<Task[]>(`${this.apiUrl}/all?id=${id}`);
  }

  getTasksByEmployeeId() : Observable<Task[]> {
    const employeeId = this.authService.getId();

    return this.http.get<Task[]>(`${this.apiUrl}/get_employee_tasks?id=${employeeId}`)
  }

  getFreeTasks() : Observable<Task[]> {
    const projectId = this.projectService.getProjectId();

    return this.http.get<Task[]>(`${this.apiUrl}/get_free_tasks?id=${projectId}`)
  }

  addTask(task : Task) : Observable<any> {
    task.creatorId = this.authService.getId();
    task.projectId = this.projectService.getProjectId()!;
    task.employeeId = task.creatorId;

    return this.http.post<any>(`${this.apiUrl}/create`, task);
  }

  addTaskToEmployee(taskId : string) : Observable<any> {
    const employeeId = this.authService.getId();

    return this.http.post<any>(`${this.apiUrl}/add`,{ employeeId, taskId });
  }
  
  removeEmployeeFromTask(employeeId : string,taskId : string) : Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/remove`,{ employeeId, taskId });
  }
}
