import { Injectable } from '@angular/core';
import { AuthService } from '../services/auth/auth.service';
import { Observable, map, of } from 'rxjs';
import { PartService } from '../services/part/part.service';

@Injectable({
  providedIn: 'root'
})
export class PartGuard {

  constructor(private authService : AuthService,
              private partService : PartService) {}

  canActivate(): Observable<boolean> {
    const id = this.authService.getId();

    const partId = this.partService.getPartId();

    if (partId !== null){
      return this.partService.getPartById(partId).pipe(
        map((part) => {
          return part.leaderIds?.includes(id) || this.authService.isAdmin();
        })
      );
    } else {
      return of(false);
    }
  }
}
