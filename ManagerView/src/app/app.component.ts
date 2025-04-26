import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth/auth.service';
import { PartLinksService } from './services/part-links/part-links.service';
import { MemberService } from './services/member/member.service';
import { PartService } from './services/part/part.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    standalone: false
})

export class AppComponent implements OnInit {
  employeeProfileString = 'Создайте профиль сотрудника';
  isEmployeeExist : boolean = false;
  isProjectManager : boolean = false;
  isDepartmentManager : boolean = false;

  get companyName() {
    const name = this.partService.getPartName();

    if (name !== null)
      return name;

    if(!this.authService.isAdmin())
      return 'Ожидайте распределения в компанию';
    return 'Выберите компанию';
  }

  get departmentName() {
    const name = this.partLinksService.getDepartmentName();

    if (name !== null && name !== '')
      return name;

    if(!this.authService.isAdmin())
      return 'Ожидайте распределения в отдел';
    return 'Выберите отдел'
  }

  get projectName() {
    const name = this.partService.getPartName();

    if (name !== null && name !== '')
      return name;

    if(!this.authService.isAdmin() && !this.isDepartmentManager)
      return 'Ожидайте распределения в проект';
    return 'Выберите проект';
  }

  constructor (public authService: AuthService,
               public partService : PartService,
               public partLinksService : PartLinksService,
               public memberService : MemberService) {}

  ngOnInit(): void {
    const id = this.authService.getId();

    if (id !== null ) {
      this.memberService.getEmployeeById(id).subscribe({
        next: (employee) => {
          if (employee.lastName !== null && employee.firstName !== null) {
            this.employeeProfileString = employee.lastName + ' ' + employee.firstName;
            this.isEmployeeExist = true;

            if(employee.companyId !== null && employee.companyId !== undefined) {
              this.partService.setPartId(employee.companyId);
              if (employee.companyName !== "" && employee.companyName !== undefined)
                this.partService.setPartName(employee.companyName);
            }
          }
        },
        error: () => {
          this.employeeProfileString = 'Создайте профиль сотрудника';
          this.isEmployeeExist = false;
        }
      });
    }

    const departmentId = this.partLinksService.getDepartmentId();

    if (departmentId !== null) {
      this.partLinksService.getPart(departmentId).subscribe({
        next: (department) => {
          if (department.managerId !== null && department.managerId !== undefined && department.managerId == id)
            this.isDepartmentManager = true;
        },
        error: () => this.isDepartmentManager = false
      });
    }
  }

  async logout() {
    await this.authService.logout();
  }
}
