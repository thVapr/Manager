import { Injectable } from '@angular/core';
import { AuthService } from '../services/auth/auth.service';
import { CompanyDepartmentsService } from '../services/company-departments/company-departments.service';
import { Observable, map, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DepartmentGuard {

  constructor(private authService : AuthService,
              private departmentService : CompanyDepartmentsService) {}

  canActivate(): Observable<boolean> {
    const id = this.authService.getId();

    const departmentId = this.departmentService.getDepartmentId();

    if (departmentId !== null){
      return this.departmentService.getDepartment(departmentId).pipe(
        map((department) => {
          return department.managerId === id || this.authService.isAdmin();
        })
      );
    } else {
      return of(false);
    }
  }
}
