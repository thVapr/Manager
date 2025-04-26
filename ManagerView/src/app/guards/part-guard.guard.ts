import { Injectable } from '@angular/core';
import { AuthService } from '../services/auth/auth.service';
import { PartLinksService } from '../services/part-links/part-links.service';
import { Observable, map, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PartGuard {

  constructor(private authService : AuthService,
              private partLinksService : PartLinksService) {}

  canActivate(): Observable<boolean> {
    const id = this.authService.getId();

    const departmentId = this.partLinksService.getDepartmentId();

    if (departmentId !== null){
      return this.partLinksService.getPart(departmentId).pipe(
        map((department) => {
          return department.managerId === id || this.authService.isAdmin();
        })
      );
    } else {
      return of(false);
    }
  }
}
