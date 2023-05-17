import { Component, OnInit } from '@angular/core';
import { EmployeeService } from '../services/employee/employee.service';
import { ActivatedRoute } from '@angular/router';
import { Employee } from '../models/Employee';

@Component({
  selector: 'app-employee-profile',
  templateUrl: './employee-profile.component.html',
  styleUrls: ['./employee-profile.component.scss']
})
export class EmployeeProfileComponent implements OnInit {

  employee : Employee = new Employee("","","","");
  isEmployeeProfileExist : boolean = false;

  constructor(private employeeService : EmployeeService,
              private route : ActivatedRoute) {}

  ngOnInit(): void {
    this.update();
  }
  update() : void {
    const id : string = this.route.snapshot.params['id'];
    
    this.employeeService.getEmployeeById(id)
      .subscribe({
        next: (employee) => {
          if (employee.firstName === null) {
            this.isEmployeeProfileExist = false;
            return;
          }
          
          this.isEmployeeProfileExist = true;
          this.employee = employee;
          console.log(this.employee.firstName);
          console.log(this.employee.lastName);
          console.log(this.employee.patronymic);

        },
        error: (error) => {
          console.log(error);
          this.isEmployeeProfileExist = false;
        }
      });
  }
}
