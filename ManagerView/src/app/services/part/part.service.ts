import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Part } from 'src/app/components/models/part';
import { AuthService } from '../auth/auth.service';
import { PartType } from 'src/app/components/models/part-type';
import { Constants } from 'src/app/constants';
import { TaskStatus } from 'src/app/components/models/task-status';
import { PartRole } from 'src/app/components/models/part-role';
import { PartTaskType } from 'src/app/components/models/part-task-type';

@Injectable({
  providedIn: 'root'
})
export class PartService {
  private apiUrl = Constants.SERVER_ADDRESS + '/api/parts';

  constructor(private http: HttpClient,
     private authService: AuthService) { }

  addLeader(employeeId: string | undefined) : Observable<any> {
    const id = this.getPartId();

    return this.updatePart(new Part(
      id!,
      "",
      "",
      0,
      [employeeId!],
    ));
  }

  isMemberHasPrivileges(privilege : number) : Observable<boolean> {
    const userId = this.authService.getId();
    const partId = this.getPartId();

    return this.hasPrivileges(userId!, partId!, privilege);
  }

  hasPrivileges(userId : string, partId : string, privilege : number) : Observable<boolean>
  {
    const params = new HttpParams()
      .set('privilege', privilege.toString());
    return this.http.get<boolean>(`${this.apiUrl}/${partId}/privileges/${userId}/check`, { params });
  }

  remove(partId : string) {
    return this.http.delete<any>(`${this.apiUrl}/${partId}`)
  }

  updateHierarchy(parts: Part[]) : Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/hierarchy`,parts);
  }

  getAllAccessible(): Observable<Part[]> {
    return this.http.get<Part[]>(`${this.apiUrl}/accessible`);
  }

  getAll(): Observable<Part[]> {
    const id = this.getPartId();
    return this.http.get<Part[]>(`${this.apiUrl}/${id}/all`);
  }

  getTypes(): Observable<PartType[]> {
    return this.http.get<PartType[]>(`${this.apiUrl}/types`);
  }

  getPartById(id : string): Observable<Part> {
    return this.http.get<Part>(`${this.apiUrl}/${id}`);
  }

  addRoleToPart(name : string, description : string) {
    const partId = this.getPartId();
    return this.http.post<boolean>(`${this.apiUrl}/{${partId}}/roles/`,{ PartId: partId, Name: name, Description: description });
  }

  addRoleToMember(roleId : string, memberId : string) : Observable<boolean> {
    const partId = this.getPartId();
    return this.http.post<boolean>(`${this.apiUrl}/${partId}/roles/${roleId}/members/${memberId}`, {});
  }

  removeRoleFromMember(roleId : string, memberId : string) : Observable<boolean> {
    const partId = this.getPartId();
    return this.http.delete<boolean>(`${this.apiUrl}/${partId}/roles/${roleId}/members/${memberId}`);
  }

  getRolesById(id : string): Observable<PartRole[]> {
    return this.http.get<PartRole[]>(`${this.apiUrl}/${id}/roles`);
  }

  addType(name : string) : Observable<boolean> {
    const partId = this.getPartId();
    return this.http.post<boolean>(`${this.apiUrl}/${partId}/types`, { name });
  }

  removeType(entityId : string) : Observable<boolean> {
    const partId = this.getPartId();
    return this.http.delete<boolean>(`${this.apiUrl}/${partId}/types/${entityId}`);
  }

  getTypesById(id : string): Observable<PartTaskType[]> {
    return this.http.get<PartTaskType[]>(`${this.apiUrl}/${id}/types/all`);
  }

  getMemberRolesById(partId : string, memberId : string): Observable<PartRole[]> {
    return this.http.get<PartRole[]>(`${this.apiUrl}/${partId}/roles/members/${memberId}`);
  }

  removeRoleFromPart(entityId : string) : Observable<any> {
    const partId = this.getPartId();

    return this.http.delete<any>(`${this.apiUrl}/${partId}/roles/${entityId}`);
  }
  
  addPart(name : string, description : string, typeId : number ) : Observable<any> {
    const mainPartId = this.getPartId();
    const level = 0;

    return this.http.post<any>(`${this.apiUrl}/create`, 
      { mainPartId, name, description, typeId, level });
  }

  updatePart(part: Part) : Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/update`, part);
  }


  addMemberToPart(memberId : string) : Observable<any> {
    const partId = this.getPartId();

    return this.http.post<any>(`${this.apiUrl}/${partId}/members/`,{ memberId });
  }
  
  removeMemberFromPart(memberId : string) : Observable<any> {
    const partId = this.getPartId();

    return this.http.delete<any>(`${this.apiUrl}/${partId}/members/${memberId}`);
  }
  
  setMemberPrivilege(memberId : string, privilege : number) : Observable<any> {
    const partId = this.getPartId();

    return this.http.post<any>(`${this.apiUrl}/${partId}/privileges/${memberId}`,{ privilege });
  }

  getMemberPrivilege(memberId : string) : Observable<number> {
    const partId = this.getPartId();

    return this.http.get<number>(`${this.apiUrl}/${partId}/privileges/${memberId}`);
  }

  isPartSelected() : boolean {
    const id = this.getPartId();
    const name = this.getPartName();

    return id !== null && name !== null;
  }

  setPartId(id : string) : void {
    sessionStorage.setItem('part_id',id);
  }

  getPartId() : string | null {
    const id = sessionStorage.getItem('part_id');
    
    return id;
  }

  setPartName(name : string) : void {
    sessionStorage.setItem('part_name', name);
  }

  removePartData() : void {
    sessionStorage.removeItem('part_id');
    sessionStorage.removeItem('part_name');
  }

  getPartName() : string | null {
    const name = sessionStorage.getItem('part_name');

    return name;
  }

    getPartTaskStatuses(): Observable<TaskStatus[]> {
    const id = this.getPartId();

    return this.http.get<TaskStatus[]>(`${this.apiUrl}/${id}/statuses`);
  }

  addPartStatus(taskStatus : TaskStatus) : Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/${taskStatus.partId}/statuses`, 
      taskStatus);
  }

  updatePartStatus(taskStatus: TaskStatus) : Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${taskStatus.partId}/statuses/${taskStatus.id}`, taskStatus);
  }

  removePartStatus(partTaskStatusId : string) : Observable<any> {
    const partId = this.getPartId();

    return this.http.delete<any>(`${this.apiUrl}/${partId}/statuses/${partTaskStatusId}`);
  }
}
