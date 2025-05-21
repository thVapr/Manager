import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PartService } from '../../services/part/part.service';

@Component({
    selector: 'app-part-profile',
    templateUrl: './part-profile.component.html',
    styleUrls: ['./part-profile.component.scss'],
    standalone: false
})
export class PartProfileComponent implements OnInit {
  departmentName : string | undefined = "";
  departmentDescription : string | undefined = "";

  constructor(private partService : PartService,
              private route : ActivatedRoute) {}

  ngOnInit(): void {
    const id : string = this.route.snapshot.params['id'];

    this.getPartProfile(id);
  }

  getPartProfile(id : string) : void {
    this.partService.getPartById(id).subscribe((part) => {
      this.departmentName = part.name;
      this.departmentDescription = part.description;
    });
  }
}
