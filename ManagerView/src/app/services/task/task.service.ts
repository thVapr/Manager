import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PartService } from '../part/part.service';
import { HttpClient } from '@angular/common/http';
import { Task } from 'src/app/components/models/task';
import { AuthService } from '../auth/auth.service';
import { Constants } from 'src/app/constants';
import { Member } from 'src/app/components/models/member';
import { TaskHistory } from 'src/app/components/models/task-history';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private apiUrl = Constants.SERVER_ADDRESS + '/api/tasks';

  constructor(private http: HttpClient,
              private partService : PartService,
              private authService: AuthService) { }


  getAll(): Observable<Task[]> {
    const partId = this.partService.getPartId();

    return this.http.get<Task[]>(`${this.apiUrl}/all?partId=${partId}`);
  }

  getTasksByMemberId() : Observable<Task[]> {
    const memberId = this.authService.getId();

    return this.http.get<Task[]>(`${this.apiUrl}/get_member_tasks?memberId=${memberId}`);
  }

  getMembersFromTask(taskId : string)
  {    
    return this.http.get<Member[]>(`${this.apiUrl}/get_task_members?taskId=${taskId}`);
  }

  getFreeTasks() : Observable<Task[]> {
    const partId = this.partService.getPartId();

    return this.http.get<Task[]>(`${this.apiUrl}/get_free_tasks?partId=${partId}`);
  }

  getAvailableTasks() : Observable<Task[]> {
    const partId = this.partService.getPartId();
    const memberId = this.authService.getId();

    return this.http.get<Task[]>(`${this.apiUrl}/get_available_tasks?partId=${partId}&memberId=${memberId}`);
  }

  getTaskById(taskId : string) : Observable<Task> {
    return this.http.get<Task>(`${this.apiUrl}/get?taskId=${taskId}`);
  }

  getTaskByQuery(query : string) : Observable<Task[]> {
    const partId =  this.partService.getPartId();

    return this.http.get<Task[]>(`${this.apiUrl}/search?query=${query}&partId=${partId}`);
  }

  getAvailableMembersForTask(taskId : string) : Observable<Member[]> {
    const partId =  this.partService.getPartId();

    return this.http.get<Member[]>(`${this.apiUrl}/get_available_members_for_task` +
      `?taskId=${taskId}&partId=${partId}`);
  }

  getTaskHistory(taskId : string) : Observable<TaskHistory[]> {
    return this.http.get<TaskHistory[]>(`${this.apiUrl}/get_task_history` +
      `?taskId=${taskId}`);
  }

  updateTask(name : string, description : string, task : Task) : Observable<any> {
    const partId =  this.partService.getPartId();
    task.partId = partId!;
    
    return this.http.put<any>(`${this.apiUrl}/update`, { name, description, task });
  }

  changeTaskStatus(name : string, description : string, taskId : string, forward : boolean) : Observable<boolean> {
    const partId = this.partService.getPartId();

    return this.http.patch<boolean>(
      `${this.apiUrl}/change`,
      { taskId, partId, name, description, forward }
    );
  }

  addTask(task : Task) : Observable<any> {
    task.creatorId = this.authService.getId();
    task.partId =  this.partService.getPartId()!;

    return this.http.post<any>(`${this.apiUrl}/create`, task);
  }

  addTaskToCurrentMember(taskId : string) : Observable<any> {
    const memberId = this.authService.getId();
    const groupId = 0;

    return this.addTaskToMember(memberId, taskId, groupId);
  }

  addTaskToMember(memberId : string, taskId : string, groupId : number) : Observable<any> {
    const partId = this.partService.getPartId();

    return this.http.post<any>(`${this.apiUrl}/add_member`,{ memberId, taskId, groupId, partId});
  }

  removeMemberFromTask(memberId : string, taskId : string) : Observable<any> {
    const partId = this.partService.getPartId();
    
    return this.http.post<any>(`${this.apiUrl}/remove_member`,{ memberId, taskId, partId});
  }

  removeTask(taskId : string) : Observable<any> {
    const partId = this.partService.getPartId();

    return this.http.delete<any>(`${this.apiUrl}/delete?partId=${partId}&taskId=${taskId}`);
  }
}
