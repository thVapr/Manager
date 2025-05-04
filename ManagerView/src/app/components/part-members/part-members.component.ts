import { Component } from '@angular/core';
import { MemberService } from '../../services/member/member.service';
import { Member } from '../models/member';
import { AuthService } from '../../services/auth/auth.service';
import { Part } from '../models/part';
import { PartService } from '../../services/part/part.service';
import { Router } from '@angular/router';

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

  constructor(public partService: PartService,
              public memberService: MemberService,
              public authService: AuthService,
              private router : Router) {}

  ngOnInit(): void {
    this.update();
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
}