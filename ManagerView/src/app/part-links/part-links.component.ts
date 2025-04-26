import { Router } from '@angular/router';
import { Part } from '../models/Part';
import { AuthService } from '../services/auth/auth.service';
import { Component, OnInit } from '@angular/core';
import { PartLinksService } from '../services/part-links/part-links.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { PartService } from '../services/part/part.service';

@Component({
    selector: 'app-company-departments',
    templateUrl: './part-links.component.html',
    styleUrls: ['./part-links.component.scss'],
    standalone: false
})

export class PartLinksComponent implements OnInit {
  addCompanyForm = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.maxLength(20)]),
    description: new FormControl('', [Validators.required, Validators.maxLength(20)])
  });

  departments : Part[] = [];

  constructor(public authService : AuthService,
              public partLinksService: PartLinksService,
              public router : Router,
              private partService: PartService) {}

  ngOnInit(): void {
    this.GetAll();
  }

  OnSubmit() : void {
    const name = this.addCompanyForm.value.name;
    const description = this.addCompanyForm.value.description;
    this.partLinksService.addDepartment(name!, description!)
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
      this.partLinksService.setDepartmentId(id);
      this.partLinksService.setDepartmentName(name);
      this.partService.setPartId("");
      this.partService.setPartName("");
    }

    this.router.navigate(['project']);
  }

  GetAll() : void {
    this.partLinksService.getAll().subscribe(departments => this.departments = departments);
  }
}
