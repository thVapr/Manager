import { Injectable } from '@angular/core';
import { AuthService } from '../services/auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class OwnerGuard {

  constructor(private authService : AuthService) {}
  
  canActivate() : boolean
  {
    return this.authService.hasAccess() || this.authService.isSpaceOwner() || this.authService.isAdmin();
  }

}
