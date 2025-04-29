import { Component } from '@angular/core';
import { MemberService } from '../../services/member/member.service';
import { Member } from '../models/Member';
import { AuthService } from '../../services/auth/auth.service';
import { Part } from '../models/Part';
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

  constructor(public partService: PartService,
              public memberService: MemberService,
              public authService: AuthService) {}

  ngOnInit(): void {
    this.Update();
  }

  addManager(memberId: string | undefined) {
    this.partService.addLeader(memberId).subscribe(() => {
      this.Update();
    });
  }  

  removeManager() {
    this.partService.removeLeader().subscribe(() => {
      this.Update();
    });
  }

  Update() : void {
    this.GetAll();
    this.GetAllFree();

    const id = this.partService.getPartId();

    if(id !== null)
      this.partService.getPartById(id).subscribe((part) => {
        this.part = part;
      });
  }

  AddEmployeeToDepartment(id : any) {
    this.partService.addMemberToPart(id).subscribe(() => this.Update());
  }

  RemoveEmployeeFromDepartment(id : any) {
    this.partService.removeMemberFromPart(id).subscribe(() => this.Update());
  }

  GetAllFree() : void {
    this.memberService.getMembersWithoutPart().subscribe(employees => this.members = employees);
  }

  GetAll() : void {
    this.memberService.getMemberByPartId().subscribe(employees => this.partMembers = employees);
  }
}
