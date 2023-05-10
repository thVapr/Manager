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

  get companyName() {
    const name = this.companyService.getCompanyName();

    if (name !== null)
      return name;

    return 'Выберите компанию';
  }

  get departmentName() {
    const name = this.companyDepartmentsService.getDepartmentName();

    if (name !== null && name !== '')
      return name;

    return 'Выберите отдел';
  }

  get projectName() {
    const name = this.projectService.getProjectName();

    if (name !== null && name !== '')
      return name;

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
          }
        },
        error: (error) => {
          this.employeeProfileString = 'Создайте профиль сотрудника';
          this.isEmployeeExist = false;
        }
      });
    }
  }

  async logout() {
    await this.authService.logout();
  }
}
