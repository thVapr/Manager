import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PartService } from '../part/part.service';
import { HttpClient } from '@angular/common/http';
import { Task } from 'src/app/models/Task';
import { PartLinksService } from '../part-links/part-links.service';
import { AuthService } from '../auth/auth.service';
import { Status } from 'src/app/status'

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private apiUrl = 'http://localhost:6732/api/tasks';

  constructor(private http: HttpClient,
              private partService : PartService,
              private PartLinksService: PartLinksService,
              private authService: AuthService) { }


  getAll(): Observable<Task[]> {
    const id = this.partService.getPartId();

    return this.http.get<Task[]>(`${this.apiUrl}/all?id=${id}`);
  }

  getTasksByEmployeeId() : Observable<Task[]> {
    const employeeId = this.authService.getId();

    return this.http.get<Task[]>(`${this.apiUrl}/get_employee_tasks?id=${employeeId}`);
  }

  getFreeTasks() : Observable<Task[]> {
    const projectId = this.partService.getPartId();

    return this.http.get<Task[]>(`${this.apiUrl}/get_free_tasks?id=${projectId}`);
  }

  getTaskById(id : string) : Observable<Task> {
    return this.http.get<Task>(`${this.apiUrl}/get?id=${id}`);
  }

  getTaskByQuery(query : string) : Observable<Task[]> {
    const id =  this.partService.getPartId();

    return this.http.get<Task[]>(`${this.apiUrl}/search?query=${query}&id=${id}`);
  }

  updateTask(task : Task) : Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/update`, task);
  }

  addTask(task : Task) : Observable<any> {
    task.creatorId = this.authService.getId();
    task.projectId =  this.partService.getPartId()!;
    task.employeeId = task.creatorId;

    return this.http.post<any>(`${this.apiUrl}/create`, task);
  }

  addTaskToEmployee(taskId : string) : Observable<any> {
    const employeeId = this.authService.getId();

    return this.http.post<any>(`${this.apiUrl}/add`,{ employeeId, taskId});
  }
  
  removeEmployeeFromTask(employeeId : string,taskId : string) : Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/remove`,{ employeeId, taskId });
  }
}
