import { Component } from '@angular/core';
import { PartLinksService } from '../services/part-links/part-links.service';
import { MemberService } from '../services/member/member.service';
import { Employee } from '../models/Employee';
import { AuthService } from '../services/auth/auth.service';
import { Part } from '../models/Part';

@Component({
    selector: 'app-department-employees',
    templateUrl: './part-members.component.html',
    styleUrls: ['./part-members.component.scss'],
    standalone: false
})
export class PartMembersComponent {
  employees : Employee[] = [];
  departmentEmployees: Employee[] = [];
  department : Part = new Part("","","","");

  constructor(public partLinksService: PartLinksService,
              public memberService: MemberService,
              public authService: AuthService) {}

  ngOnInit(): void {
    this.Update();
  }

  addManager(employeeId: string | undefined) {
    this.partLinksService.addManager(employeeId).subscribe(() => {
      this.Update();
    });
  }  

  removeManager() {
    this.partLinksService.removeManager().subscribe(() => {
      this.Update();
    });
  }

  Update() : void {
    this.GetAll();
    this.GetAllFree();

    const id = this.partLinksService.getDepartmentId();

    if(id !== null)
      this.partLinksService.getPart(id).subscribe((department) => {
        this.department = department;
      });
  }

  AddEmployeeToDepartment(id : any) {
    this.partLinksService.addEmployeeToDepartment(id).subscribe(() => this.Update());
  }

  RemoveEmployeeFromDepartment(id : any) {
    this.partLinksService.removeEmployeeFromDepartment(id).subscribe(() => this.Update());
  }

  GetAllFree() : void {
    this.memberService.getEmployeesWithoutDepartment().subscribe(employees => this.employees = employees);
  }

  GetAll() : void {
    this.memberService.getEmployeesByDepartmentId().subscribe(employees => this.departmentEmployees = employees);
  }
}
