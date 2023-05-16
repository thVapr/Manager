import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth/auth.service';
import { CompanyService } from './services/company/company.service';
import { CompanyDepartmentsService } from './services/company-departments/company-departments.service';
import { EmployeeService } from './services/employee/employee.service';
import { ProjectService } from './services/project/project.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})

export class AppComponent implements OnInit {
  employeeProfileString = 'Создайте профиль сотрудника';
  isEmployeeExist : boolean = false;
  isProjectManager : boolean = false;
  isDepartmentManager : boolean = false;

  get companyName() {
    const name = this.companyService.getCompanyName();

    if (name !== null)
      return name;

    if(!this.authService.isAdmin())
      return 'Ожидайте распределения в компанию';
    return 'Выберите компанию';
  }

  get departmentName() {
    const name = this.companyDepartmentsService.getDepartmentName();

    if (name !== null && name !== '')
      return name;

    if(!this.authService.isAdmin())
      return 'Ожидайте распределения в отдел';
    return 'Выберите отдел'
  }

  get projectName() {
    const name = this.projectService.getProjectName();

    if (name !== null && name !== '')
      return name;

    if(!this.authService.isAdmin() && !this.isDepartmentManager)
      return 'Ожидайте распределения в проект';
    return 'Выберите проект';
  }

  constructor (public authService: AuthService,
               public companyService : CompanyService,
               public companyDepartmentsService : CompanyDepartmentsService,
               public employeeService : EmployeeService,
               public projectService: ProjectService) {}

  ngOnInit(): void {
    const id = this.authService.getId();

    if (id !== null ) {
      this.employeeService.getEmployeeById(id).subscribe({
        next: (employee) => {
          if (employee.lastName !== null && employee.firstName !== null) {
            this.employeeProfileString = employee.lastName + ' ' + employee.firstName;
            this.isEmployeeExist = true;

            if(employee.companyId !== null && employee.companyId !== undefined) {
              this.companyService.setCompanyId(employee.companyId);
              if (employee.companyName !== "" && employee.companyName !== undefined)
                this.companyService.setCompanyName(employee.companyName);
            }
            
            if(employee.projectId !== null && employee.projectId !== undefined) {
              this.projectService.setProjectId(employee.projectId);
              if (employee.projectName !== "" && employee.projectName !== undefined)
                this.projectService.setProjectName(employee.projectName);
            }

            if(employee.departmentId !== null && employee.departmentId !== undefined) {
              this.companyDepartmentsService.setDepartmentId(employee.departmentId);
              if (employee.departmentName !== "" && employee.departmentName !== undefined)
                this.companyDepartmentsService.setDepartmentName(employee.departmentName);
            }

          }
        },
        error: () => {
          this.employeeProfileString = 'Создайте профиль сотрудника';
          this.isEmployeeExist = false;
        }
      });
    }

    const departmentId = this.companyDepartmentsService.getDepartmentId();

    if (departmentId !== null) {
      this.companyDepartmentsService.getDepartment(departmentId).subscribe({
        next: (department) => {
          if (department.managerId !== null && department.managerId !== undefined && department.managerId == id)
            this.isDepartmentManager = true;
        },
        error: () => this.isDepartmentManager = false
      });
    }

    const projectId = this.projectService.getProjectId();

    if (projectId !== null) {
      this.projectService.getProjectById(projectId).subscribe({
        next: (project) => {
          if (project.managerId !== null && project.managerId !== undefined && project.managerId == id)
            this.isProjectManager = true;
        },
        error: () => this.isProjectManager = false
      });
    }

  }

  async logout() {
    await this.authService.logout();
  }
}
