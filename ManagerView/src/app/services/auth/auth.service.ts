import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, firstValueFrom, map, of, tap } from 'rxjs';

import { jwtDecode } from 'jwt-decode'
import { Constants } from 'src/app/constants';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = Constants.SERVER_ADDRESS + '/api/authentication';

  constructor(private http: HttpClient) {}

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
    const decoded : any = jwtDecode(token!);

    return decoded.role;
  }

  getId() : string {
    const token = this.getAccessToken();
    const decoded : any = jwtDecode(token!);

    return decoded.id;
  }

  isAdmin() : boolean {
    const role = this.getRole();
    
    return role === 'Admin';
  }

  isSpaceOwner() : boolean {
    const role = this.getRole();

    return role === 'SpaceOwner';
  }

  hasAccess() : boolean {
    return true;
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
  
    const tokenData = jwtDecode(token) as any;
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

  logout(): Observable<void> {
    const refreshToken = localStorage.getItem('refresh_token');

    return this.http.post<any>(`${this.apiUrl}/logout`, { refreshToken }).pipe(
      tap(() => localStorage.clear()),
      catchError((error) => {
        localStorage.removeItem('access_token');
        localStorage.removeItem('refresh_token');
        return of();
      }),
      map(() => void 0)
    );
  }
}
