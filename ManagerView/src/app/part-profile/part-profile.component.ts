import { Component, OnInit } from '@angular/core';
import { PartLinksService } from '../services/part-links/part-links.service';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-department-profile',
    templateUrl: './part-profile.component.html',
    styleUrls: ['./part-profile.component.scss'],
    standalone: false
})
export class PartProfileComponent implements OnInit {
  departmentName : string | undefined = "";
  departmentDescription : string | undefined = "";

  constructor(private partLinksService : PartLinksService,
              private route : ActivatedRoute) {}

  ngOnInit(): void {
    const id : string = this.route.snapshot.params['id'];

    this.getDepartmentProfile(id);
  }

  getDepartmentProfile(id : string) : void {
    this.partLinksService.getPart(id).subscribe((department) => {
      this.departmentName = department.name;
      this.departmentDescription = department.description;
    });
  }
}
