import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Part } from 'src/app/components/models/part';
import { AuthService } from '../auth/auth.service';
import { PartType } from 'src/app/components/models/part-type';
import { Constants } from 'src/app/constants';
import { TaskStatus } from 'src/app/components/models/task-status';
import { PartRole } from 'src/app/components/models/part-role';

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

  isLeaderExist(leadersId : string[] | undefined): boolean {
    if (leadersId?.length != 0)
      return true;
    return false;
  }
  
  isMemberHasPrivileges(privilege : number) : Observable<boolean> {
    const userId = this.authService.getId();
    const partId = this.getPartId();

    return this.hasPrivileges(userId!, partId!, privilege);
  }

  isLeader(id : string | undefined, leadersId : string[] | undefined) : boolean {
    if (this.isLeaderExist(leadersId) && leadersId?.includes(id!)) {
      return true;
    }
    return false;    
  }

  hasPrivileges(userId : string, partId : string, privilege : number) : Observable<boolean>
  {
    const params = new HttpParams()
      .set('userId', userId)
      .set('partId', partId)
      .set('privilege', privilege.toString());
    return this.http.get<boolean>(`${this.apiUrl}/check_privileges`, { params });
  }

  removeLeader() : Observable<any> {
    const id = this.getPartId();
    const emptyId = "00000000-0000-0000-0000-000000000000";

    return this.updatePart(new Part(
      id!,
      "",
      "",
      0,
      [emptyId],
    ));
  }

  updateHierarchy(parts: Part[]) : Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/update_hierarchy`,parts);
  }

  getAllAccessible(): Observable<Part[]> {
    return this.http.get<Part[]>(`${this.apiUrl}/all_accessible`);
  }

  getAll(): Observable<Part[]> {
    const id = this.getPartId();
    return this.http.get<Part[]>(`${this.apiUrl}/all?partId=${id}`);
  }

  getTypes(): Observable<PartType[]> {
    return this.http.get<PartType[]>(`${this.apiUrl}/get_types`);
  }

  getPartById(id : string): Observable<Part> {
    return this.http.get<Part>(`${this.apiUrl}/get?partId=${id}`);
  }

  addRoleToPart(name : string, description : string) {
    const partId = this.getPartId();
    return this.http.post<boolean>(`${this.apiUrl}/roles/add_role`,{ PartId: partId, Name: name, Description: description });
  }

  addRoleToMember(roleId : string, memberId : string) : Observable<boolean> {
    const partId = this.getPartId();
    return this.http.post<boolean>(`${this.apiUrl}/roles/add_member`, { partId, roleId, memberId });
  }

  removeRoleFromMember(roleId : string, memberId : string) : Observable<boolean> {
    const partId = this.getPartId();
    return this.http.post<boolean>(`${this.apiUrl}/roles/remove_member`, { partId, roleId, memberId });
  }

  getRolesById(id : string): Observable<PartRole[]> {
    return this.http.get<PartRole[]>(`${this.apiUrl}/roles/get_roles?partId=${id}`);
  }

  getMemberRolesById(partId : string, memberId : string): Observable<PartRole[]> {
    return this.http.get<PartRole[]>(`${this.apiUrl}/roles/get_member_roles?partId=${partId}&memberId=${memberId}`);
  }

  removeRoleFromPart(roleId : string) : Observable<any> {
    const partId = this.getPartId();

    return this.http.post<any>(`${this.apiUrl}/roles/remove_role`,{ partId, roleId });
  }
  
  getPartTaskStatuses(): Observable<TaskStatus[]> {
    const id = this.getPartId();
    return this.http.get<TaskStatus[]>(`${this.apiUrl}/get_part_statuses?partId=${id}`);
  }

  addPart(name : string, description : string, typeId : number ) : Observable<any> {
    const mainPartId = this.getPartId();
    const level = 0;

    return this.http.post<any>(`${this.apiUrl}/create`, 
      { mainPartId, name, description, typeId, level });
  }

  updatePart(project: Part) : Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/update`, project);
  }

  addPartStatus(taskStatus : TaskStatus) : Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/statuses/create`, 
      taskStatus);
  }

  updatePartStatus(taskStatus: TaskStatus) : Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/statuses/update`, taskStatus);
  }

  removePartStatus(partTaskStatusId : string) : Observable<any> {
    const partId = this.getPartId();

    return this.http.delete<any>(`${this.apiUrl}/statuses/remove?partId=${partId}&partTaskStatusId=${partTaskStatusId}`);
  }

  addMemberToPart(memberId : string) : Observable<any> {
    const partId = this.getPartId();

    return this.http.post<any>(`${this.apiUrl}/add`,{ partId, memberId });
  }
  
  removeMemberFromPart(memberId : string) : Observable<any> {
    const partId = this.getPartId();

    return this.http.post<any>(`${this.apiUrl}/remove`,{ partId, memberId });
  }
  
  setMemberPrivilege(memberId : string, privilege : number) : Observable<any> {
    const partId = this.getPartId();

    return this.http.post<any>(`${this.apiUrl}/change_privilege`,{partId, memberId, privilege});
  }

  getMemberPrivilege(memberId : string) : Observable<number> {
    const partId = this.getPartId();

    return this.http.get<number>(`${this.apiUrl}/get_privileges?partId=${partId}&memberId=${memberId}`);
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
}
