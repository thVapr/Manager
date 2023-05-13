import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { EmployeeService } from '../services/employee/employee.service';
import { AuthService } from '../services/auth/auth.service';

@Component({
  selector: 'app-employee',
  templateUrl: './employee.component.html',
  styleUrls: ['./employee.component.scss']
})
export class EmployeeComponent implements OnInit {
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

  constructor(public employeeService : EmployeeService, private authService : AuthService) {}
  
  ngOnInit(): void {
    const id = this.authService.getId();
    
    this.employeeService.getEmployeeById(id)
      .subscribe({
        next: (employee) => {
          if (employee.firstName === null) {
            this.isEmployeeProfileExist = false;
            return;
          }
          
          this.isEmployeeProfileExist = true;

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

    this.employeeService.addEmployee(id, firstName!, lastName!, patronymic!)
    .subscribe({
      next: () => {
        console.log('successful')
      },
      error: (error) => console.error('failed', error)
      });
  }

}
