import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MemberService } from '../services/member/member.service';
import { AuthService } from '../services/auth/auth.service';
import { Employee } from '../models/Employee';

@Component({
    selector: 'app-employee',
    templateUrl: './member.component.html',
    styleUrls: ['./member.component.scss'],
    standalone: false
})
export class MemberComponent implements OnInit {

  employee: Employee = new Employee("","","","");
  
  addEmployeeForm = new FormGroup({
    firstName: new FormControl('', [Validators.required, Validators.maxLength(20)]),
    lastName: new FormControl('', [Validators.required, Validators.maxLength(20)]),
    patronymic: new FormControl('', [Validators.required, Validators.maxLength(20)])
  });

  changeEmployeeForm = new FormGroup({
    firstName: new FormControl('', [Validators.required, Validators.maxLength(20)]),
    lastName: new FormControl('', [Validators.required, Validators.maxLength(20)]),
    patronymic: new FormControl('', [Validators.required, Validators.maxLength(20)])
  });

  public isEmployeeProfileExist : boolean = false;

  constructor(public memberService : MemberService, private authService : AuthService) {}
  
  ngOnInit(): void {
    this.update();
  }

  update() : void {
    const id = this.authService.getId();
    
    this.memberService.getEmployeeById(id)
      .subscribe({
        next: (employee) => {
          if (employee.firstName === null) {
            this.isEmployeeProfileExist = false;
            return;
          }
          
          this.isEmployeeProfileExist = true;

          this.employee = employee;
          this.changeEmployeeForm.setValue({
            firstName: employee.firstName!,
            lastName: employee.lastName!,
            patronymic: employee.patronymic!,
          });
        },
        error: (error) => {
          console.log(error);
          this.isEmployeeProfileExist = false;
        }
      });
  }

  onAddSubmit() : void {
    const id = this.authService.getId();
    const firstName = this.addEmployeeForm.value.firstName;
    const lastName = this.addEmployeeForm.value.lastName;
    const patronymic = this.addEmployeeForm.value.patronymic;

    this.memberService.addEmployee(id, firstName!, lastName!, patronymic!)
    .subscribe({
      next: () => {
        console.log('successful')
        this.update();
      },
      error: (error) => console.error('failed', error)
      });
  }

  onChangeSubmit() : void {
    const id = this.authService.getId();
    const firstName = this.changeEmployeeForm.value.firstName;
    const lastName = this.changeEmployeeForm.value.lastName;
    const patronymic = this.changeEmployeeForm.value.patronymic;

    this.memberService.updateEmployee(id, firstName!, lastName!, patronymic!)
    .subscribe({
      next: () => {
        console.log('successful')
        this.update();
      },
      error: (error) => console.error('failed', error)
      });
  }

}
