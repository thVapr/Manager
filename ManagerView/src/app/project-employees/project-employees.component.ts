import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Employee } from '../models/Employee';
import { ProjectService } from '../services/project/project.service';
import { EmployeeService } from '../services/employee/employee.service';

@Component({
  selector: 'app-project-employees',
  templateUrl: './project-employees.component.html',
  styleUrls: ['./project-employees.component.scss']
})
export class ProjectEmployeesComponent implements OnInit{
  employees : Employee[] = [];
  projectEmployees: Employee[] = [];

  constructor(public projectService: ProjectService,
              public employeeService: EmployeeService) {}

  ngOnInit(): void {
    this.Update();
  }

  OnChoose(id : string | undefined) : void {

  }

  Update() : void {
    this.GetAll();
    this.GetAllFree();
  }

  AddEmployeeToProject(id : any) {
    this.projectService.addEmployeeToProject(id).subscribe(() => this.Update());
  }

  RemoveEmployeeFromProject(id : any) {
    this.projectService.removeEmployeeFromProject(id).subscribe(() => this.Update());
  }

  GetAllFree() : void {
    this.employeeService.getEmployeesWithoutProject().subscribe(employees => this.employees = employees);
  }

  GetAll() : void {
    this.employeeService.getEmployeesByProjectId().subscribe(employees => this.projectEmployees = employees);
  }
}
