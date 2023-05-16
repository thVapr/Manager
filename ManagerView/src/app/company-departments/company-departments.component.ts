import { Router } from '@angular/router';
import { Department } from '../models/Department';
import { AuthService } from '../services/auth/auth.service';
import { Component, OnInit } from '@angular/core';
import { CompanyDepartmentsService } from '../services/company-departments/company-departments.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ProjectService } from '../services/project/project.service';

@Component({
  selector: 'app-company-departments',
  templateUrl: './company-departments.component.html',
  styleUrls: ['./company-departments.component.scss']
})

export class CompanyDepartmentsComponent implements OnInit {
  addCompanyForm = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.maxLength(20)]),
    description: new FormControl('', [Validators.required, Validators.maxLength(20)])
  });

  departments : Department[] = [];

  constructor(public authService : AuthService,
              public companyDepartmentsService: CompanyDepartmentsService,
              public router : Router,
              private projectService: ProjectService) {}

  ngOnInit(): void {
    this.GetAll();
  }

  OnSubmit() : void {
    const name = this.addCompanyForm.value.name;
    const description = this.addCompanyForm.value.description;
    this.companyDepartmentsService.addDepartment(name!, description!)
    .subscribe({
      next: () => {
       console.log('successful');
       this.GetAll();
       this.addCompanyForm.reset();
      },
      error: (error) => console.error('failed', error)
     });
  }

  ChooseDepartment(id : any, name : any) : void {
    if (id !== undefined && name !== undefined) {
      this.companyDepartmentsService.setDepartmentId(id);
      this.companyDepartmentsService.setDepartmentName(name);
      this.projectService.setProjectId("");
      this.projectService.setProjectName("");
    }

    this.router.navigate(['home']);
  }

  GetAll() : void {
    this.companyDepartmentsService.getAll().subscribe(departments => this.departments = departments);
  }
}
