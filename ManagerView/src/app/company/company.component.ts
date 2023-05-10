import { Component, OnInit } from '@angular/core';
import { Company } from '../models/Company'
import { CompanyService } from '../services/company/company.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../services/auth/auth.service';
import { Router } from '@angular/router';
import { CompanyDepartmentsService } from '../services/company-departments/company-departments.service';
import { ProjectService } from '../services/project/project.service';


@Component({
  selector: 'app-company',
  templateUrl: './company.component.html',
  styleUrls: ['./company.component.scss']
})
export class CompanyComponent implements OnInit {
  addCompanyForm = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.maxLength(20)]),
    description: new FormControl('', [Validators.required, Validators.maxLength(20)])
  });

  companies: Company[] = [];

  constructor(public companyService: CompanyService,
              public companyDepartmentsService : CompanyDepartmentsService,
              public authService: AuthService, private router: Router,
              private projectService: ProjectService) {}

  ngOnInit(): void {
      this.GetAll();
  }

  OnSubmit() : void {
    const name = this.addCompanyForm.value.name;
    const description = this.addCompanyForm.value.description;
    this.companyService.addCompany(name!, description!)
      .subscribe({
        next: () => {
        console.log('successful')
        this.GetAll()
        },
        error: (error) => console.error('failed', error)
      });
  }

  ChooseCompany(id : any, name : any) : void {
    if (id !== undefined && name !== undefined) {
      this.companyService.setCompanyId(id);
      this.companyService.setCompanyName(name);

      this.companyDepartmentsService.removeDepatmentData();
      this.projectService.removeProjectData();
    }

    this.router.navigate(['company/departments']);
  }

  GetAll() : void {
    this.companyService.getAll()
      .subscribe(companies => this.companies = companies);
  }
}
