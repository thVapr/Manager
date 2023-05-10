import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth/auth.service';
import { CompanyService } from './services/company/company.service';
import { CompanyDepartmentsService } from './services/company-departments/company-departments.service';
import { EmployeeService } from './services/employee/employee.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})

export class AppComponent implements OnInit {
  title = 'Добро пожаловать';

  get companyName() {
    const name = this.companyService.getCompanyName();

    if (name !== null)
      return name;

    return 'Выберите компанию';
  }

  get departmentName() {
    const name = this.companyDeparmentsService.getDepartmentName();

    if (name !== null && name !== '')
      return name;

    return 'Выберите отдел';
  }

  constructor (public authService: AuthService,
               public companyService : CompanyService,
               public companyDeparmentsService : CompanyDepartmentsService,
               public employeeService : EmployeeService) {}

  ngOnInit(): void {
  }

  async logout() {
    await this.authService.logout();
  }
}
