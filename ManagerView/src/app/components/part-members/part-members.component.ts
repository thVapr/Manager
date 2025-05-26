import { Component } from '@angular/core';
import { MemberService } from '../../services/member/member.service';
import { Member } from '../models/member';
import { AuthService } from '../../services/auth/auth.service';
import { Part } from '../models/part';
import { PartService } from '../../services/part/part.service';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { PartRole } from '../models/part-role';
import { PRIVILEGE_LABELS } from '../privilege-labels';

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

  addManager(memberId: string | undefined) {
    this.partService.addLeader(memberId).subscribe(() => {
      this.update();
    });
  }  

  removeManager() {
    this.partService.removeLeader().subscribe(() => {
      this.update();
    });
  }

  doubleClick(member : Member)
  {
    this.router.navigate(["/member/about/", member.id]);
  }

  update() : void {
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
  }

  removeRole(id : string) : void {
    this.partService.removeRoleFromPart(id).subscribe({
      next: () => this.update()
    });
  }

  addEmployeeToDepartment(id : any) {
    this.partService.addMemberToPart(id).subscribe(() => this.update());
  }

  removeEmployeeFromDepartment(id : any) {
    this.partService.removeMemberFromPart(id).subscribe(() => this.update());
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