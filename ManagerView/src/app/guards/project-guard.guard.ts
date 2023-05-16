import { Injectable } from '@angular/core';
import { Observable, map, of, switchMap } from 'rxjs';
import { AuthService } from '../services/auth/auth.service';
import { CompanyDepartmentsService } from '../services/company-departments/company-departments.service';
import { ProjectService } from '../services/project/project.service';

@Injectable({
  providedIn: 'root'
})
export class ProjectGuard {

  constructor(private authService : AuthService,
    private departmentService : CompanyDepartmentsService,
    private projectService : ProjectService) {}

    canActivate(): Observable<boolean> {
      const id = this.authService.getId();
      const departmentId = this.departmentService.getDepartmentId();
      const projectId = this.projectService.getProjectId();
  
      if (departmentId !== null) {
        return this.departmentService.getDepartment(departmentId).pipe(
          switchMap((department) => {
            if (projectId !== null) {
              return this.projectService.getProjectById(projectId).pipe(
                map((project) => {
                  return department.managerId === id || project.managerId === id || this.authService.isAdmin();
                })
              );
            } else {
              return of(false);
            }
          })
        );
      } else {
        return of(false);
      }
    }
  
}
