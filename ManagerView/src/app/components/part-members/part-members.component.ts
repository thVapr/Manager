import { Router } from '@angular/router';
import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { Part } from '../models/part';
import { Member } from '../models/member';
import { PartRole } from '../models/part-role';
import { PRIVILEGE_LABELS } from '../privilege-labels';
import { PartTaskType } from '../models/part-task-type';
import { MemberService } from '../../services/member/member.service';
import { AuthService } from '../../services/auth/auth.service';
import { PartService } from '../../services/part/part.service';

@Component({
    selector: 'app-part-members',
    templateUrl: './part-members.component.html',
    styleUrls: ['./part-members.component.scss'],
    standalone: false
})
export class PartMembersComponent {
  members : Member[] = [];
  partMembers: Member[] = [];
  part : Part = new Part("","","",0,[]);
  privelegeOptions = PRIVILEGE_LABELS;

  addRoleForm = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.minLength(1), Validators.maxLength(20)]),
    description: new FormControl('', [Validators.maxLength(20)]),
  });
  roles : PartRole[] = [];
  addTypeForm = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.minLength(1), Validators.maxLength(20)]),
  });
  types : PartTaskType[] = [];

  constructor(public partService: PartService,
              public memberService: MemberService,
              public authService: AuthService,
              private router : Router) {}

  ngOnInit(): void {
    this.update();
  }
  
  onSubmit() : void {
    if (this.addRoleForm === null)
      return;
    this.partService.addRoleToPart(this.addRoleForm.value.name!, this.addRoleForm.value.description!)
      .subscribe({
        next: () => {
          this.update();
        }
      }
    );
  }

  onTypeSubmit() : void {
    if (this.addTypeForm === null)
      return;
    console.log(this.addTypeForm.value.name!);
    this.partService.addType(this.addTypeForm.value.name!)
      .subscribe({
        next: () => {
          this.update();
        }
      });
  }
  
  onMoveToTarget(event : any)
  {
    event.items.forEach((element: Member) => {
      this.partService.removeMemberFromPart(element.id!).subscribe({next: () => {
        this.update();
      }});
    });
  }

  onMoveToSource(event : any)
  {
    event.items.forEach((element: Member) => {
      this.partService.addMemberToPart(element.id!).subscribe({next: () => {
        this.update();
      }});
    });
  }

  doubleClick(member : Member)
  {
    this.router.navigate(["/member/about/", member.id]);
  }

  private update() : void {
    this.addRoleForm.reset();
    this.addTypeForm.reset();
    this.getAll();
    this.getAllFree();

    this.partService.getRolesById(this.partService.getPartId()!)
      .subscribe({
        next: (roles) => {
          this.roles = roles; 
        },
        error: () => {
          this.roles = [];
        }
    });
    this.partService.getTypesById(this.partService.getPartId()!)
      .subscribe({
        next: (types) => {
          this.types = types; 
        },
        error: () => {
          this.types = [];
        }
    });
  }

  removeRole(id : string) : void {
    this.partService.removeRoleFromPart(id).subscribe({
      next: () => this.update()
    });
  }

  removeType(id : string) : void {
    this.partService.removeType(id).subscribe({
      next: () => this.update()
    });
  }

  getAllFree() : void {
    this.memberService.getAvailableMembers().subscribe({
      next: (members) => {
        this.members = members.map(member => ({
          ...member,
          searchedParameter: member.firstName + ' ' + member.lastName! + ' ' + member.patronymic!
        }));
      },
      error: () => {
        this.members = []
      }
    });
  }

  getAll(): void {
      this.memberService.getMembersByPartId().subscribe(members => {
          const completedRequests = new Array(members.length).fill(false);
          
          members.forEach((member, index) => {
              this.partService.getMemberPrivilege(member.id!).subscribe({
                  next: (privilege) => {
                      member.privilege = privilege;
                      completedRequests[index] = true;
                      
                      if (completedRequests.every(Boolean)) {
                          this.updatePartMembers(members);
                      }
                  },
                  error: () => {
                      member.privilege = undefined;
                      completedRequests[index] = true;
                      
                      if (completedRequests.every(Boolean)) {
                          this.updatePartMembers(members);
                      }
                  }
              });
          });
          
          if (members.length === 0) {
              this.updatePartMembers(members);
          }
      });
  }

  private updatePartMembers(members: Member[]): void {
      this.partMembers = members.map(member => ({
          ...member,
          searchedParameter: `${member.firstName} ${member.lastName} ${member.patronymic}`
      }));
  }

  getPrivilageLabelByValue(value : number) {
    return this.privelegeOptions.find(privelege => privelege.value === value)?.label;
  }
}