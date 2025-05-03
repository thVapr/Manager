import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth/auth.service';
import { MemberService } from './services/member/member.service';
import { PartService } from './services/part/part.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
    standalone: false
})

export class AppComponent implements OnInit {
  memberProfileString : string = 'Создайте профиль сотрудника';
  isMemberExist : boolean = false;
  isPartLeader : boolean = false;
  isMainPartLeader : boolean = false;

  get partName() {
    const name = this.partService.getPartName();

    if (name !== null)
      return name;

    return 'Выберите сущность';
  }

  constructor (public authService: AuthService,
               public partService : PartService,
               public memberService : MemberService) {}

  ngOnInit(): void {
    const id = this.authService.getId();

    if (id !== null ) {
      this.memberService.getMemberById(id).subscribe({
        next: (member) => {
          if (member.lastName !== null && member.firstName !== null) {
            this.memberProfileString = member.lastName + ' ' + member.firstName;
            this.isMemberExist = true;

            if(member.partId !== null && member.partId !== undefined) {
              this.partService.setPartId(member.partId);
              if (member.partName !== "" && member.partName !== undefined)
                this.partService.setPartName(member.partName);
            }
          }
        },
        error: () => {
          this.memberProfileString = 'Создайте профиль сотрудника';
          this.isMemberExist = false;
        }
      });
    }

    const partId = this.partService.getPartId();

    if (partId !== null) {
      this.partService.getPartById(partId).subscribe({
        next: (part) => {
          if (part.leaderIds?.includes(id))
            this.isMainPartLeader = true;
        },
        error: () => this.isMainPartLeader = false
      });
    }
  }

  async logout() {
    await this.authService.logout();
  }
}
