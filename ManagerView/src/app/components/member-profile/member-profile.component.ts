import { Component, OnInit } from '@angular/core';
import { MemberService } from '../../services/member/member.service';
import { ActivatedRoute } from '@angular/router';
import { Member } from '../models/Member';

@Component({
    selector: 'app-employee-profile',
    templateUrl: './member-profile.component.html',
    styleUrls: ['./member-profile.component.scss'],
    standalone: false
})
export class MemberProfileComponent implements OnInit {

  employee : Member = new Member("","","","");
  isEmployeeProfileExist : boolean = false;

  constructor(private memberService : MemberService,
              private route : ActivatedRoute) {}

  ngOnInit(): void {
    this.update();
  }
  update() : void {
    const id : string = this.route.snapshot.params['id'];
    
    this.memberService.getMemberById(id)
      .subscribe({
        next: (employee) => {
          if (employee.firstName === null) {
            this.isEmployeeProfileExist = false;
            return;
          }
          
          this.isEmployeeProfileExist = true;
          this.employee = employee;
          console.log(this.employee.firstName);
          console.log(this.employee.lastName);
          console.log(this.employee.patronymic);

        },
        error: (error) => {
          console.log(error);
          this.isEmployeeProfileExist = false;
        }
      });
  }
}
