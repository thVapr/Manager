import { Injectable } from '@angular/core';
import { AuthService } from '../services/auth/auth.service';
import { Observable, map, of } from 'rxjs';
import { PartService } from '../services/part/part.service';
import { ActivatedRouteSnapshot } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class PartGuard {

  constructor(private authService : AuthService,
              private partService : PartService) {}

  canActivate(route: ActivatedRouteSnapshot): Observable<boolean> {
    const id = this.authService.getId();
    const partId = this.partService.getPartId();
    const privilege = route.data['privilege'] || 5;
    console.log(privilege);
    if (partId !== null){
      return this.partService.hasPrivileges(id, partId, privilege).pipe(
        map((response) => {
          return response || this.authService.isAdmin() || this.authService.isSpaceOwner();
        })
      );
    } else {
      return of(false);
    }
  }
}
