import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from 'src/app/components/models/member';
import { AuthService } from '../auth/auth.service';
import { PartService } from '../part/part.service';
import { Constants } from 'src/app/constants';

@Injectable({
  providedIn: 'root'
})
export class MemberService {
  private apiUrl = Constants.SERVER_ADDRESS + '/api/members';

  constructor(private http: HttpClient, private authService : AuthService,
              private partService : PartService) { }

  getMemberById(id : string): Observable<Member> {
    return this.http.get<Member>(`${this.apiUrl}/${id}/profile`);
  }

  addMember(id : string, firstName? : string, lastName? : string, patronymic? : string) : Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/`, {id, firstName, lastName, patronymic});
  }

  updateMember(id : string, firstName? : string, lastName? : string, patronymic? : string, messengerId? : string) : Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/`, {id, firstName, lastName, patronymic,messengerId});
  }

  isCurrentMember(employeeId : string | undefined) {
    const id = this.authService.getId();

    return employeeId === id;
  }

  getMembersByPartId() : Observable<Member[]> {
    const partId = this.partService.getPartId();
    
    return this.http.get<Member[]>(`${this.apiUrl}/all?partId=${partId}`);
  }

  getAvailableMembers() : Observable<Member[]> {
    const partId = this.partService.getPartId();

    return this.http.get<Member[]>(`${this.apiUrl}/available?partId=${partId}`);
  }
}
