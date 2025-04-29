import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Part } from 'src/app/components/models/Part';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class PartService {
  private apiUrl = 'http://localhost:6732/api/parts';

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
  
  isLeader(id : string | undefined, leadersId : string[] | undefined) : boolean {
    if (this.isLeaderExist(leadersId) && leadersId?.includes(id!)) {
      return true;
    }
    return false;    
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

    return this.http.get<Part[]>(`${this.apiUrl}/all?id=${id}`);
  }

  getPartById(id : string): Observable<Part> {
    return this.http.get<Part>(`${this.apiUrl}/get?id=${id}`);
  }

  addPart(name : string, description : string, typeId : number ) : Observable<any> {
    const departmentId = this.getPartId();
    const managerId = this.authService.getId();
    const level = 0;

    return this.http.post<any>(`${this.apiUrl}/create`, 
      { departmentId, name, description, managerId, typeId, level });
  }

  updatePart(project: Part) : Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/update`, project);
  }

  addMemberToPart(employeeId : string) : Observable<any> {
    const projectId = this.getPartId();

    return this.http.post<any>(`${this.apiUrl}/add`,{ projectId, employeeId });
  }
  
  removeMemberFromPart(employeeId : string) : Observable<any> {
    const projectId = this.getPartId();

    return this.http.post<any>(`${this.apiUrl}/remove`,{ projectId, employeeId });
  }
  
  isPartSelected() : boolean {
    const id = this.getPartId();
    const name = this.getPartName();

    return id !== null && name !== null;
  }

  setPartId(id : string) : void {
    localStorage.setItem('part_id',id);
  }

  getPartId() : string | null {
    const id = localStorage.getItem('part_id');
    
    return id;
  }

  setPartName(name : string) : void {
    localStorage.setItem('part_name', name);
  }

  removePartData() : void {
    localStorage.removeItem('part_id');
    localStorage.removeItem('part_name');
  }

  getPartName() : string | null {
    const name = localStorage.getItem('part_name');

    return name;
  }
}
