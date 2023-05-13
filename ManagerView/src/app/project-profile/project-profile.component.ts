import { Component, OnInit } from '@angular/core';
import { ProjectService } from '../services/project/project.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-project-profile',
  templateUrl: './project-profile.component.html',
  styleUrls: ['./project-profile.component.scss']
})
export class ProjectProfileComponent implements OnInit {
  projectName : string | undefined = "";
  projectDescription : string | undefined = "";

  constructor(private projectService : ProjectService,
              private route : ActivatedRoute) {}

  ngOnInit(): void {
    const id : string = this.route.snapshot.params['id'];

    this.getProjectProfile(id);
  }

  getProjectProfile(id : string) : void {
    this.projectService.getProjectById(id).subscribe((project) => {
      this.projectName = project.name;
      this.projectDescription = project.description;
    });
  }
}
