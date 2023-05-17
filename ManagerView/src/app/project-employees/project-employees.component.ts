import { Component, OnInit } from '@angular/core';
import { Employee } from '../models/Employee';
import { ProjectService } from '../services/project/project.service';
import { EmployeeService } from '../services/employee/employee.service';
import { Project } from '../models/Project';
import { AuthService } from '../services/auth/auth.service';
import { CompanyDepartmentsService } from '../services/company-departments/company-departments.service';
import { Department } from '../models/Department';

@Component({
  selector: 'app-project-employees',
  templateUrl: './project-employees.component.html',
  styleUrls: ['./project-employees.component.scss']
})
export class ProjectEmployeesComponent implements OnInit{
  project: Project = new Project("","","","");
  department: Department = new Department("","","","");
  employees : Employee[] = [];
  projectEmployees: Employee[] = [];
  isManager : boolean = false;

  constructor(public projectService: ProjectService,
              public employeeService: EmployeeService,
              public authService: AuthService,
              public departmentService: CompanyDepartmentsService) {}

  ngOnInit(): void {
    this.Update();
  }

  addManager(employeeId: string | undefined) {
    this.projectService.addManager(employeeId).subscribe(() => {
      this.Update();
    });
  }  

  removeManager() {
    this.projectService.removeManager().subscribe(() => {
      this.Update();
    });
  }
  
  Update() : void {
    this.GetAll();
    this.GetAllFree();

    const id = this.projectService.getProjectId();

    if(id !== null)
      this.projectService.getProjectById(id).subscribe((project) => {
        this.project = project;
      });

    const departmentId = this.departmentService.getDepartmentId()

    if(departmentId !== null)
      this.departmentService.getDepartment(departmentId).subscribe((department) => {
        this.department = department;

        const id = this.authService.getId();

        if (department.managerId !== null && department.managerId == id)
          this.isManager = true;
        else
          this.isManager = false;
      });
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
