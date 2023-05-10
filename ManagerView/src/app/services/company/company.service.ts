import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, map } from 'rxjs';
import { Company } from 'src/app/models/Company';

@Injectable({
  providedIn: 'root'
})
export class CompanyService {
  private apiUrl = 'http://localhost:5106/api/company';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Company[]> {
    return this.http.get<Company[]>(`${this.apiUrl}/all`);
  }

  addCompany(name: string, description: string) : Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/create`, { name, description });
  }
  
  setCompanyId(id : string) : void {
    localStorage.setItem('company_id',id);
  }

  setCompanyName(name : string) : void {
    localStorage.setItem('company_name', name);
  }

  getCompanyName() : string | null {
    const name = localStorage.getItem('company_name');
    return name;
  }

  getCompanyId() : string | null {
    const id = localStorage.getItem('company_id');
    return id;
  }

  getCompany(id : string) : Observable<Company> {
    return this.http.get<Company>(`${this.apiUrl}/get?id={id}`);
  }

  isCompanySelected() : boolean {
    const id = this.getCompanyId();
    const name = this.getCompanyName();

    return id !== null && name !== null;
  }
}
