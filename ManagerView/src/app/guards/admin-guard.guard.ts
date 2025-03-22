import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard {
  
  constructor(private authService : AuthService) {}
  
  canActivate() : boolean
  {
    return this.authService.isAdmin();
  }
}
