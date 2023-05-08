import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, firstValueFrom, tap } from 'rxjs';

import jwt_decode from 'jwt-decode'

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5106/api/authentication';

  constructor(private http: HttpClient) {}

  getData(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/data`);
  }

  register(email: string, password: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/register`, { email, password }).pipe(
      tap(response => {
        const { item1, item2 } = response;
        localStorage.setItem('access_token', item1);
        localStorage.setItem('refresh_token', item2);
      })
    );
  }

  login(email: string, password: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, { email, password}).pipe(
      tap(response => {
        const {item1, item2} = response;
        localStorage.setItem('access_token', item1);
        localStorage.setItem('refresh_token', item2);
      })
    );
  }

  getAccessToken(): string | null {
    return localStorage.getItem('access_token');
  }

  getRefreshToken(): string | null {
    return localStorage.getItem('refresh_token');
  }

  setAccessToken(accessToken: string): void {
    localStorage.setItem('access_token', accessToken);
  }

  setRefreshToken(refreshToken: string): void {
    localStorage.setItem('refresh_token', refreshToken);
  }

  getRole() : string {
    const token = this.getAccessToken();
    const decoded : any = jwt_decode(token!);

    return decoded.role;
  }

  isAdmin() : boolean {
    const role = this.getRole();

    return role === 'Admin';
  }

  isAuthenticated(): boolean {
    const access_token = this.getAccessToken();
    const refresh_token = this.getRefreshToken();

    return access_token != null && refresh_token != null;
  }

  getTokenExpiration(): Date {
    const token = this.getAccessToken();

    if (!token) {
      return new Date(0);
    }
  
    const tokenData = jwt_decode(token) as any;
    const expirationDate = new Date(0);

    expirationDate.setUTCSeconds(tokenData.exp);
    
    return expirationDate;
  }
  
  refresh() {
    const accessToken = this.getAccessToken();
    const refreshToken = this.getRefreshToken();

    return this.http.post<any>(`${this.apiUrl}/refresh`, { accessToken, refreshToken }).pipe(
      tap(response => {
        const {item1, item2} = response;
        localStorage.setItem('access_token', item1);
        localStorage.setItem('refresh_token', item2);
      })
    );
  }

  async logout() {
    const refreshToken = localStorage.getItem('refresh_token');

    await firstValueFrom(this.http.post<any>(`${this.apiUrl}/logout`, { refreshToken }));

    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
  }
}
