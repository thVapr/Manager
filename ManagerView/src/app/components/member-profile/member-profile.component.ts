import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { Member } from '../models/member';
import { PartService } from 'src/app/services/part/part.service';
import { MemberService } from '../../services/member/member.service';

@Component({
    selector: 'app-employee-profile',
    templateUrl: './member-profile.component.html',
    styleUrls: ['./member-profile.component.scss'],
    standalone: false
})
export class MemberProfileComponent implements OnInit {

  isMemberHasPermissionsToEdit : boolean = false;
  changeEmployeeForm = new FormGroup({
    firstName: new FormControl('', [Validators.required, Validators.maxLength(20)]),
    lastName: new FormControl('', [Validators.required, Validators.maxLength(20)]),
    patronymic: new FormControl('', [Validators.required, Validators.maxLength(20)]),
    privilege: new FormControl(0, [Validators.required])
  });
  member : Member = new Member("","","","");
  isEmployeeProfileExist : boolean = false;

  constructor(private memberService : MemberService,
              private route : ActivatedRoute,
              public partService : PartService) {}

  ngOnInit(): void {
    this.checkMemberPermissions();
    this.update();
  }

  onChangeSubmit(): void {
    const id = this.member.id;
    const firstName = this.changeEmployeeForm.value.firstName;
    const lastName = this.changeEmployeeForm.value.lastName;
    const patronymic = this.changeEmployeeForm.value.patronymic;
    const privilege = this.changeEmployeeForm.value.privilege;

    this.memberService.updateMember(id!, firstName!, lastName!, patronymic!)
    .subscribe({
      next: () => {
        this.partService.setMemberPrivilege(id!, privilege!).subscribe({
          next: () => {
            console.log('successful');
            this.update();
          }
        });
      },
      error: (error) => console.error('failed', error)
      });
  }

  private checkMemberPermissions() : void {
    this.partService.isMemberHasPrivileges(5).subscribe({
      next: (response) => { 
        this.isMemberHasPermissionsToEdit = response;
      }
    });
  }

  private update() : void {
    const id : string = this.route.snapshot.params['id'];
    
    this.memberService.getMemberById(id)
      .subscribe({
        next: (member) => {
          if (member.firstName === null) {
            this.isEmployeeProfileExist = false;
            return;
          }
          this.partService.getMemberPrivilege(member.id!).subscribe({
            next: (privilege) => {
              member.privilege = privilege;
              this.isEmployeeProfileExist = true;
              this.member = member;
          
              this.changeEmployeeForm.setValue({
                firstName: member.firstName!,
                lastName: member.lastName!,
                patronymic: member.patronymic!,
                privilege: member.privilege!
              });
            }
          });
        },
        error: (error) => {
          this.isEmployeeProfileExist = false;
        }
      });
  }
}
