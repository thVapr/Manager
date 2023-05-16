import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ProjectService } from '../services/project/project.service';
import { Router } from '@angular/router';
import { Project } from 'src/app/models/Project'
import { AuthService } from '../services/auth/auth.service';
import { CompanyDepartmentsService } from '../services/company-departments/company-departments.service';

@Component({
  selector: 'app-project',
  templateUrl: './project.component.html',
  styleUrls: ['./project.component.scss']
})
export class ProjectComponent {
  addProjectForm = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.maxLength(20)]),
    description: new FormControl('', [Validators.required, Validators.maxLength(20)])
  });

  isDepartmentManager : boolean = false;
  projects : Project[] = [];

  constructor(public authService: AuthService,
              public projectService: ProjectService,
              public router : Router,
              public departmentService: CompanyDepartmentsService) {}

  ngOnInit(): void {
    this.GetAll();

    const departmentId = this.departmentService.getDepartmentId();
    const id = this.authService.getId();

    if (departmentId !== null && id !== null) {
      this.departmentService.getDepartment(departmentId).subscribe({
        next: (department) => {
          if (department.managerId !== null && department.managerId !== undefined && department.managerId == id)
            this.isDepartmentManager = true;
        },
        error: () => this.isDepartmentManager = false
      });
    }
  }

  OnSubmit() : void {
    const name = this.addProjectForm.value.name;
    const description = this.addProjectForm.value.description;
    this.projectService.addProject(name!, description!)
    .subscribe({
      next: () => {
       console.log('successful')
       this.GetAll()
      },
      error: (error) => console.error('failed', error)
     });
  }

  ChooseProject(id : any, name : any) : void {
    if (id !== undefined && name !== undefined) {
      this.projectService.setProjectId(id);
      this.projectService.setProjectName(name);
    }

    this.router.navigate(['home']);

  }

  GetAll() : void {
    this.projectService.getAll().subscribe(projects => this.projects = projects);
  }
}
