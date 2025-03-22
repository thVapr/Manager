import { Component, OnInit } from '@angular/core';
import { CompanyDepartmentsService } from '../services/company-departments/company-departments.service';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-department-profile',
    templateUrl: './department-profile.component.html',
    styleUrls: ['./department-profile.component.scss'],
    standalone: false
})
export class DepartmentProfileComponent implements OnInit {
  departmentName : string | undefined = "";
  departmentDescription : string | undefined = "";

  constructor(private departmentService : CompanyDepartmentsService,
              private route : ActivatedRoute) {}

  ngOnInit(): void {
    const id : string = this.route.snapshot.params['id'];

    this.getDepartmentProfile(id);
  }

  getDepartmentProfile(id : string) : void {
    this.departmentService.getDepartment(id).subscribe((department) => {
      this.departmentName = department.name;
      this.departmentDescription = department.description;
    });
  }
}
