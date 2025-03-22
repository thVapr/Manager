import { Component, OnInit } from '@angular/core';
import { CompanyService } from '../services/company/company.service';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-company-profile',
    templateUrl: './company-profile.component.html',
    styleUrls: ['./company-profile.component.scss'],
    standalone: false
})
export class CompanyProfileComponent implements OnInit {

  companyName : string | undefined = "";
  companyDescription : string | undefined = "";

  constructor(private companyService : CompanyService,
              private route : ActivatedRoute) {}

  ngOnInit(): void {
    const id : string = this.route.snapshot.params['id'];

    this.getCompanyProfile(id);
  }

  getCompanyProfile(id : string) : void {
    this.companyService.getCompany(id).subscribe((company) => {
      this.companyName = company.name;
      this.companyDescription = company.description;
    });
  }
}
