import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PartService } from '../part/part.service';
import { HttpClient } from '@angular/common/http';
import { Task } from 'src/app/components/models/task';
import { AuthService } from '../auth/auth.service';
import { Status } from 'src/app/status'
import { Constants } from 'src/app/constants';
import { Member } from 'src/app/components/models/member';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private apiUrl = Constants.SERVER_ADDRESS + '/api/tasks';

  constructor(private http: HttpClient,
              private partService : PartService,
              private authService: AuthService) { }


  getAll(): Observable<Task[]> {
    const id = this.partService.getPartId();

    return this.http.get<Task[]>(`${this.apiUrl}/all?id=${id}`);
  }

  getTasksByMemberId() : Observable<Task[]> {
    const memberId = this.authService.getId();

    return this.http.get<Task[]>(`${this.apiUrl}/get_member_tasks?id=${memberId}`);
  }

  getMembersFromTask(taskId : string)
  {    
    return this.http.get<Member[]>(`${this.apiUrl}/get_task_members?taskId=${taskId}`);
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

  changeTaskStatus(taskId : string) : Observable<boolean> {
    return this.http.patch<boolean>(`${this.apiUrl}/change?taskId=${taskId}`, null);
  }

  addTask(task : Task) : Observable<any> {
    task.creatorId = this.authService.getId();
    task.partId =  this.partService.getPartId()!;

    return this.http.post<any>(`${this.apiUrl}/create`, task);
  }

  addTaskToMember(taskId : string) : Observable<any> {
    const memberId = this.authService.getId();
    const groupId = 0;
    
    return this.http.post<any>(`${this.apiUrl}/add`,{ memberId, taskId, groupId});
  }
  
  removeEmployeeFromTask(employeeId : string,taskId : string) : Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/remove`,{ employeeId, taskId });
  }
}
