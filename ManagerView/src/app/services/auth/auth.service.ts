import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, firstValueFrom, tap } from 'rxjs';

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
        sessionStorage.setItem('access_token', item1);
        sessionStorage.setItem('refresh_token', item2);
      })
    );
  }

  login(email: string, password: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/login`, { email, password}).pipe(
      tap(response => {
        const {item1, item2} = response;
        sessionStorage.setItem('access_token', item1);
        sessionStorage.setItem('refresh_token', item2);
      })
    );
  }

  getAccessToken(): string | null {
    return sessionStorage.getItem('access_token');
  }

  getRefreshToken(): string | null {
    return sessionStorage.getItem('refresh_token');
  }

  setAccessToken(accessToken: string): void {
    sessionStorage.setItem('access_token', accessToken);
  }

  setRefreshToken(refreshToken: string): void {
    sessionStorage.setItem('refresh_token', refreshToken);
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
        sessionStorage.setItem('access_token', item1);
        sessionStorage.setItem('refresh_token', item2);
      })
    );
  }

  async logout() : Promise<void> {
    try {      
      const refreshToken = sessionStorage.getItem('refresh_token');

      await firstValueFrom(this.http.post<any>(`${this.apiUrl}/logout`, { refreshToken }));
      sessionStorage.clear();
 
    } catch (error) {
      sessionStorage.removeItem('access_token');
      sessionStorage.removeItem('refresh_token');
    }
  }
}
