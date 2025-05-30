import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PartService } from '../part/part.service';
import { HttpClient } from '@angular/common/http';
import { Task } from 'src/app/components/models/task';
import { AuthService } from '../auth/auth.service';
import { Constants } from 'src/app/constants';
import { Member } from 'src/app/components/models/member';
import { TaskHistory } from 'src/app/components/models/task-history';
import { TaskMessage } from 'src/app/components/models/task-message';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private partId = this.partService.getPartId();
  private apiUrl = Constants.SERVER_ADDRESS + `/api/parts/${this.partId}/tasks`;

  constructor(private http: HttpClient,
              private partService : PartService,
              private authService: AuthService) { }


  getAll(): Observable<Task[]> {
    return this.http.get<Task[]>(`${this.apiUrl}/all`);
  }

  getTasksByMemberId() : Observable<Task[]> {
    const memberId = this.authService.getId();

    return this.http.get<Task[]>(`${this.apiUrl}/members/assigned?memberId=${memberId}`);
  }

  getMembersFromTask(taskId : string)
  {    
    return this.http.get<Member[]>(`${this.apiUrl}/${taskId}/members`);
  }

  getFreeTasks() : Observable<Task[]> {
    return this.http.get<Task[]>(`${this.apiUrl}/free`);
  }

  getAvailableTasks() : Observable<Task[]> {
    const memberId = this.authService.getId();

    return this.http.get<Task[]>(`${this.apiUrl}/available?memberId=${memberId}`);
  }

  getTaskById(taskId : string) : Observable<Task> {
    return this.http.get<Task>(`${this.apiUrl}?taskId=${taskId}`);
  }

  getTaskByQuery(query : string) : Observable<Task[]> {
    return this.http.get<Task[]>(`${this.apiUrl}/search?query=${query}`);
  }

  getAvailableMembersForTask(taskId : string) : Observable<Member[]> {
    return this.http.get<Member[]>(`${this.apiUrl}/${taskId}/members/available`);
  }

  getTaskHistory(taskId : string) : Observable<TaskHistory[]> {
    return this.http.get<TaskHistory[]>(`${this.apiUrl}/history` +
      `?taskId=${taskId}`);
  }

  getMessages(taskId : string) : Observable<TaskMessage[]> {
    return this.http.get<TaskMessage[]>(`${this.apiUrl}/${taskId}/messages`);
  }

  addMessage(message : TaskMessage) : Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/${message.taskId}/messages/`, message);
  }

  removeMessage(taskId : string, messageId : string) : Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${taskId}/messages/?messageId=${messageId}`);
  }

  getFileList(taskId : string) : Observable<string[]> {
    return this.http.get<string[]>(`${this.apiUrl}/${taskId}/files`);
  }

  getFile(fileName : string,taskId : string) : Observable<Blob> {
    return this.http.get(`${this.apiUrl}/${taskId}/files/${fileName}`,
    {
      responseType: 'blob'
    });
  }

  addFile(file : File, taskId : string) : Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    
    return this.http.post<any>(`${this.apiUrl}/${taskId}/files`, formData);
  }

  updateTask(name : string, description : string, task : Task) : Observable<any> {
    const partId =  this.partService.getPartId();
    task.partId = partId!;
    
    return this.http.put<any>(`${this.apiUrl}/${task.id!}`, { name, description, task });
  }

  changeTaskStatus(name : string, description : string, taskId : string, forward : boolean) : Observable<boolean> {
    return this.http.patch<boolean>(
      `${this.apiUrl}/${taskId}`,
      { name, description, forward }
    );
  }

  addTask(task : Task) : Observable<any> {
    task.creatorId = this.authService.getId();
    task.partId =  this.partService.getPartId()!;
    if (task.taskTypeId === undefined || task.taskTypeId === "")
      task.taskTypeId = null!;

    return this.http.post<any>(`${this.apiUrl}`, task);
  }

  addTaskToCurrentMember(taskId : string) : Observable<any> {
    const memberId = this.authService.getId();
    const groupId = 0;

    return this.addTaskToMember(memberId, taskId, groupId);
  }

  addTaskToMember(memberId : string, taskId : string, groupId : number) : Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/${taskId}/members`,{ memberId, groupId});
  }

  removeMemberFromTask(memberId : string, taskId : string) : Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${taskId}/members`, { params: { memberId} });
  }

  removeTask(taskId : string) : Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${taskId}`);
  }
}