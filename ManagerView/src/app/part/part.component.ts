import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { PartService } from '../services/part/part.service';
import { Router } from '@angular/router';
import { Project } from 'src/app/models/Project'
import { AuthService } from '../services/auth/auth.service';
import { PartLinksService } from '../services/part-links/part-links.service';

@Component({
    selector: 'app-project',
    templateUrl: './part.component.html',
    styleUrls: ['./part.component.scss'],
    standalone: false
})
export class PartComponent {
  addProjectForm = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.maxLength(20)]),
    description: new FormControl('', [Validators.required, Validators.maxLength(20)])
  });

  isDepartmentManager : boolean = false;
  projects : Project[] = [];

  constructor(public authService: AuthService,
              public partService: PartService,
              public router : Router,
              public partLinksService: PartLinksService) {}

  ngOnInit(): void {
    this.GetAll();

    const departmentId = this.partLinksService.getDepartmentId();
    const id = this.authService.getId();

    if (departmentId !== null && id !== null) {
      this.partLinksService.getPart(departmentId).subscribe({
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
    this.partService.addProject(name!, description!)
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
      this.partService.setPartId(id);
      this.partService.setPartName(name);
    }

    this.router.navigate(['home']);

  }

  GetAll() : void {
    this.partService.getAll().subscribe(projects => this.projects = projects);
  }
}
