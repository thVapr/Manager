import { Component, HostListener, OnInit } from '@angular/core';
import { AuthService } from './services/auth/auth.service';
import { MemberService } from './services/member/member.service';
import { PartService } from './services/part/part.service';
import { Router } from '@angular/router';
import { UpdateService } from './services/update.service';
import { finalize } from 'rxjs';

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
  items : CustomMenuItem[] = [];

  isCollapsed = true;

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    if (!(event.target as Element).closest('.navbar-collapse') && 
        !(event.target as Element).closest('.navbar-toggler')) {
      this.isCollapsed = true;
    }
  }

  get partName() {
    const name = this.partService.getPartName();

    if (name !== null)
      return name;

    return 'Выберите сущность';
  }

  constructor (public authService: AuthService,
               public partService : PartService,
               public memberService : MemberService,
               public router : Router,
               private updateService: UpdateService) {}

  ngOnInit(): void {
    this.updateService.startConnection();
    this.updateMenuItems();
    const id = this.authService.getId();
    if (id !== null ) {
      this.memberService.getMemberById(id).subscribe({
        next: (member) => {
          if (member.lastName !== null && member.firstName !== null) {
            this.memberProfileString = member.lastName + ' ' + member.firstName;
            this.isMemberExist = true;
            const partId = this.partService.getPartId();

            if(partId !== null) {
              this.partService.hasPrivileges(id, partId, 5).subscribe({
                next: (response) => {
                  this.isPartLeader = response;
                  this.updateMenuItems();
                },
                error: () => this.isPartLeader = false
              });
            }
          }
          this.updateMenuItems();
        },
        error: () => {
          this.memberProfileString = 'Создайте профиль сотрудника';
          this.isMemberExist = false;
          this.updateMenuItems();
        }
      });
    }
  }

  logout() : void {
    this.authService.logout()
    .pipe(
      finalize( () => {
        this.router.navigate(['/login']);
        this.updateMenuItems();
      }
    ))
    .subscribe({
      next: () => {
      }
    });
  }

  executeCommand(event: Event, item: any) {
    if (item.command) {
        event.preventDefault();
    }
    
    if (typeof item.command === 'function') {
        item.command({
            originalEvent: event,
            item: item
        });
    }
    
    if (!item.routerLink && !item.command) {
        event.preventDefault();
    }
    if (window.innerWidth < 992) {
      this.isCollapsed = true;
    }
  }

  updateMenuItems() : void {
    this.items = [];

    if (!this.authService.isAuthenticated()) {
      this.items.push(
        {
          label: 'Вход',
          icon: 'bi bi-box-arrow-in-right',
          routerLink: 'login'
        },
        {
          label: 'Регистрация',
          icon: 'bi bi-r-circle',
          routerLink: 'register'
        }
      );
    }

    if (this.authService.isAuthenticated()) {
      const item : CustomMenuItem = {
        label: this.memberProfileString,
        icon: 'bi bi-person-lines-fill',
        items: [
          {        
            label: 'Профиль',
            icon: 'bi bi-file-person',
            routerLink: 'member/about/' + this.authService.getId()
          },
          {        
            label: 'Выйти',
            icon: 'bi bi-box-arrow-left',
            command: () => this.logout()
          }
        ]
      };
      this.items.push(item);

      if (this.isMemberExist && 
        (this.authService.isAdmin()||this.authService.isSpaceOwner()||this.authService.hasAccess())) {
        this.items.push({
          label: this.partName,
          icon: 'bi bi-person-workspace',
          routerLink: 'parts'
        });
      }
    }

    if (this.authService.isAuthenticated()) {
      if ((this.authService.isAdmin() || this.isPartLeader) && this.partService.isPartSelected()) {
        this.items.push({
          label: 'Управление',
          icon: 'bi bi-file-person',
          routerLink: 'part/members'
        });
        if (false)
        {
          this.items.push({
            label: 'Профиль',
            icon: 'bi bi-cup-hot',
            routerLink: 'part/about'
          });
        }
      }

      if (this.partService.isPartSelected()) {
        this.items.push({
          label: 'Статистика',
          icon: 'bi bi-house',
          routerLink: 'home'
        });
        this.items.push({
          label: 'Задачи',
          icon: 'bi bi-list-task',
          routerLink: 'member/tasks'
        });

        this.items.push({
          label: 'Задачи проекта',
          icon: 'bi bi-kanban',
          routerLink: 'part/tasks'
        });
      }

      if (window.innerWidth < 992) {
        this.isCollapsed = true;
      }
    }
  }
}

class CustomMenuItem {
  public label? : string;
  public icon? : string;
  public routerLink? : string;
  public command? : (() => void);
  public items? : CustomMenuItem[];
}