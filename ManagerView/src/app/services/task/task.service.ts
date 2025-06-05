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
import { TaskFile } from 'src/app/components/models/task-file';

@Injectable({
  providedIn: 'root'
})
export class TaskService {

  private getApiUrl = () : string =>
     Constants.SERVER_ADDRESS + `/api/parts/${this.partService.getPartId()}/tasks`;

  constructor(private http: HttpClient,
              private partService : PartService,
              private authService: AuthService) { }


  getAll(): Observable<Task[]> {
    return this.http.get<Task[]>(`${this.getApiUrl()}/all`);
  }

  getTasksByMemberId() : Observable<Task[]> {
    const memberId = this.authService.getId();

    return this.http.get<Task[]>(`${this.getApiUrl()}/members/assigned?memberId=${memberId}`);
  }

  getMembersFromTask(taskId : string)
  {    
    return this.http.get<Member[]>(`${this.getApiUrl()}/${taskId}/members`);
  }

  getFreeTasks() : Observable<Task[]> {
    return this.http.get<Task[]>(`${this.getApiUrl()}/free`);
  }

  getAvailableTasks() : Observable<Task[]> {
    const memberId = this.authService.getId();

    return this.http.get<Task[]>(`${this.getApiUrl()}/available?memberId=${memberId}`);
  }

  getTaskById(taskId : string) : Observable<Task> {
    return this.http.get<Task>(`${this.getApiUrl()}?taskId=${taskId}`);
  }

  getTaskByQuery(query : string) : Observable<Task[]> {
    return this.http.get<Task[]>(`${this.getApiUrl()}/search?query=${query}`);
  }

  getAvailableMembersForTask(taskId : string) : Observable<Member[]> {
    return this.http.get<Member[]>(`${this.getApiUrl()}/${taskId}/members/available`);
  }

  getTaskHistory(taskId : string) : Observable<TaskHistory[]> {
    return this.http.get<TaskHistory[]>(`${this.getApiUrl()}/history` +
      `?taskId=${taskId}`);
  }

  getMessages(taskId : string) : Observable<TaskMessage[]> {
    return this.http.get<TaskMessage[]>(`${this.getApiUrl()}/${taskId}/messages`);
  }

  addMessage(message : TaskMessage) : Observable<any> {
    return this.http.post<any>(`${this.getApiUrl()}/${message.taskId}/messages/`, message);
  }

  removeMessage(taskId : string, messageId : string) : Observable<any> {
    return this.http.delete<any>(`${this.getApiUrl()}/${taskId}/messages/?messageId=${messageId}`);
  }

  getFileList(taskId : string) : Observable<TaskFile[]> {
    return this.http.get<TaskFile[]>(`${this.getApiUrl()}/${taskId}/files`);
  }

  getFile(fileName : string, taskId : string) : Observable<Blob> {
    return this.http.get(`${this.getApiUrl()}/${taskId}/files/${fileName}`,
    {
      responseType: 'blob'
    });
  }

  addFile(file : File, taskId : string) : Observable<any> {
    const formData = new FormData();
    formData.append('file', file);
    
    return this.http.post<any>(`${this.getApiUrl()}/${taskId}/files`, formData);
  }

  removeFile(fileName : string, taskId : string) : Observable<any> {
    return this.http.delete<any>(`${this.getApiUrl()}/${taskId}/files/${fileName}`);
  }

  updateTask(name : string, description : string, task : Task) : Observable<any> {
    const partId =  this.partService.getPartId();
    task.partId = partId!;
    
    return this.http.put<any>(`${this.getApiUrl()}/${task.id!}`, { name, description, task });
  }

  changeTaskStatus(name : string, description : string, taskId : string, forward : boolean) : Observable<boolean> {
    return this.http.patch<boolean>(
      `${this.getApiUrl()}/${taskId}`,
      { name, description, forward }
    );
  }

  addTask(task : Task) : Observable<any> {
    task.creatorId = this.authService.getId();
    task.partId =  this.partService.getPartId()!;
    if (task.taskTypeId === undefined || task.taskTypeId === "")
      task.taskTypeId = null!;

    return this.http.post<any>(`${this.getApiUrl()}`, task);
  }

  addTaskToCurrentMember(taskId : string) : Observable<any> {
    const memberId = this.authService.getId();
    const groupId = 0;

    return this.addTaskToMember(memberId, taskId, groupId);
  }

  addTaskToMember(memberId : string, taskId : string, groupId : number) : Observable<any> {
    return this.http.post<any>(`${this.getApiUrl()}/${taskId}/members`,{ memberId, groupId});
  }

  removeMemberFromTask(memberId : string, taskId : string) : Observable<any> {
    return this.http.delete<any>(`${this.getApiUrl()}/${taskId}/members`, { params: { memberId} });
  }

  removeTask(taskId : string) : Observable<any> {
    return this.http.delete<any>(`${this.getApiUrl()}/${taskId}`);
  }
}