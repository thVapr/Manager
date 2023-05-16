import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard {

  constructor(private authService : AuthService,
              private router : Router) {}

  canActivate(): boolean {
    
    if (this.authService.isAuthenticated())
      return true;
    
    this.router.navigate(['/login']);
    return false;
  }
  
}
