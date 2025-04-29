import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from 'src/app/components/models/Member';
import { AuthService } from '../auth/auth.service';
import { PartService } from '../part/part.service';

@Injectable({
  providedIn: 'root'
})
export class MemberService {
  private apiUrl = 'http://localhost:6732/api/members';

  constructor(private http: HttpClient, private authService : AuthService,
              private partService : PartService) { }

  getMemberById(id : string): Observable<Member> {
    return this.http.get<Member>(`${this.apiUrl}/get_member_profile?id=${id}`);
  }

  addMember(id : string, firstName? : string, lastName? : string, patronymic? : string) : Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/create`, {id, firstName, lastName, patronymic});
  }

  updateMember(id : string, firstName? : string, lastName? : string, patronymic? : string) : Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/update`, {id, firstName, lastName, patronymic});
  }

  isCurrentMember(employeeId : string | undefined) {
    const id = this.authService.getId();

    return employeeId === id;
  }

  getMemberByPartId() : Observable<Member[]> {
    const id = this.partService.getPartId();
    
    return this.http.get<Member[]>(`${this.apiUrl}/all?id=${id}`);
  }

  getMembersWithoutPart() : Observable<Member[]> {
    return this.http.get<Member[]>(`${this.apiUrl}/get_free_members`);
  }
}
