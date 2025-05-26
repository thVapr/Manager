import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { Member } from '../models/member';
import { PartService } from 'src/app/services/part/part.service';
import { MemberService } from '../../services/member/member.service';
import { PartRole } from '../models/part-role';
import { PRIVILEGE_LABELS } from '../privilege-labels';
import { AuthService } from 'src/app/services/auth/auth.service';

@Component({
  selector: 'app-employee-profile',
  templateUrl: './member-profile.component.html',
  styleUrls: ['./member-profile.component.scss'],
  standalone: false
})
export class MemberProfileComponent implements OnInit {
  privilegeOptions = PRIVILEGE_LABELS;
  isMemberHasPermissionsToEdit: boolean = false;
  isCurrentMember : boolean = false;

  changeEmployeeForm = new FormGroup({
    firstName: new FormControl('', [Validators.required, Validators.maxLength(20)]),
    lastName: new FormControl('', [Validators.required, Validators.maxLength(20)]),
    patronymic: new FormControl('', [Validators.required, Validators.maxLength(20)]),
    privilege: new FormControl<number>(0, [Validators.required]),
    roles: new FormControl<PartRole[]>([], []),
    messengerId: new FormControl('', [])
  });

  addEmployeeForm = new FormGroup({
    firstName: new FormControl('', [Validators.required, Validators.maxLength(20)]),
    lastName: new FormControl('', [Validators.required, Validators.maxLength(20)]),
    patronymic: new FormControl('', [Validators.required, Validators.maxLength(20)])
  });

  member: Member = new Member("", "", "", "");
  isEmployeeProfileExist: boolean = false;
  roles: PartRole[] = [];
  previousSelection: PartRole[] = [];
  rolesToAdd: PartRole[] = [];
  rolesToRemove: PartRole[] = [];

  constructor(
    private memberService: MemberService,
    private route: ActivatedRoute,
    public partService: PartService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.update();
  }

  onChangeSubmit(): void {
    const id = this.member.id;
    const firstName = this.changeEmployeeForm.value.firstName;
    const lastName = this.changeEmployeeForm.value.lastName;
    const patronymic = this.changeEmployeeForm.value.patronymic;
    const privilege = this.changeEmployeeForm.value.privilege;
    const filtered = this.removeIntersectionFromBoth(this.rolesToAdd, this.rolesToRemove);
    const messengerId = this.changeEmployeeForm.value.messengerId;

    this.memberService.updateMember(id!, firstName!, lastName!, patronymic!, messengerId!)
      .subscribe({
        next: () => {
          this.partService.setMemberPrivilege(id!, privilege!).subscribe({
            next: () => {
              filtered.partRolesToAdd.forEach(role => {
                this.partService.addRoleToMember(role.id!, id!).subscribe({next: () => {}});
              })
              filtered.partRolesToRemove.forEach(role => {
                this.partService.removeRoleFromMember(role.id!, id!).subscribe({next: () => {}});
              })
              
              this.update();
            }
          });
        },
        error: (error) => console.error('Ошибка обновления:', error)
      });
  }

  private update(): void {
    const id: string = this.route.snapshot.params['id'];
    this.rolesToAdd = [];
    this.rolesToRemove = [];

    this.memberService.getMemberById(id).subscribe({
      next: (member) => {
        console.log(member);
        if (!member.firstName) {
          this.isEmployeeProfileExist = false;
          return;
        }
        this.isEmployeeProfileExist = true;
        this.isCurrentMember = this.member.id === this.authService.getId();
        this.changeEmployeeForm.patchValue({
          firstName: member.firstName,
          lastName: member.lastName,
          patronymic: member.patronymic,
          privilege: member.privilege,
          messengerId: member.messengerId!
        });
        this.partService.getMemberPrivilege(member.id!).subscribe({
          next: (privilege) => {
            this.member = member;
            this.partService.isMemberHasPrivileges(5).subscribe({
              next: (response) => {
                this.isMemberHasPermissionsToEdit = response;
                if (!response) {
                  this.changeEmployeeForm.get('roles')?.disable();
                  this.changeEmployeeForm.get('privilege')?.disable();
                }
              }
            });
            member.privilege = privilege;
            this.changeEmployeeForm.patchValue({
              privilege: member.privilege,
            });

            this.partService.getRolesById(this.partService.getPartId()!)
              .subscribe({
                next: (roles) => {
                  this.roles = roles;

                  this.partService.getMemberRolesById(this.partService.getPartId()!, member.id!)
                    .subscribe({
                      next: (memberRoles) => {
                        this.previousSelection = [...memberRoles];
                        this.changeEmployeeForm.patchValue({ roles: memberRoles });
                      }
                    });
                },
                error: () => this.roles = []
              });
          }
        });
        
      },
      error: (error) => {
        this.isEmployeeProfileExist = false;
        console.error('Ошибка загрузки профиля:', error);
      }
    });
  }

  onAddSubmit() : void {
    const id = this.authService.getId();
    const firstName = this.addEmployeeForm.value.firstName;
    const lastName = this.addEmployeeForm.value.lastName;
    const patronymic = this.addEmployeeForm.value.patronymic;

    this.memberService.addMember(id, firstName!, lastName!, patronymic!)
    .subscribe({
      next: () => {
        this.update();
      },
      error: (error) => console.error('failed', error)
      });
  }

  onRoleChange(event: any): void {
    const currentRoles = this.changeEmployeeForm.get('roles')?.value || [];

    const addedRoles = currentRoles.filter(role =>
      !this.previousSelection.some(prev => prev.id === role.id)
    );

    const removedRoles = this.previousSelection.filter(prev =>
      !currentRoles.some(role => role.id === prev.id)
    );

    if (addedRoles.length > 0) this.rolesToAdd.push(...addedRoles);
    if (removedRoles.length > 0) this.rolesToRemove.push(...removedRoles);

    this.previousSelection = [...currentRoles];
  }

  private removeIntersectionFromBoth(first: PartRole[], second: PartRole[]) {
    const firstIds = new Map<string,number>();
    const secondIds = new Map<string,number>();

    first.forEach( role => {
        let existingCount = firstIds.get(role.id!);
        if (existingCount === undefined)
          firstIds.set(role.id!, 1);
        else
          firstIds.set(role.id!, ++existingCount);
      }
    );

    second.forEach( role => {
        let existingCount = secondIds.get(role.id!);
        if (existingCount === undefined)
          secondIds.set(role.id!, 1);
        else
          secondIds.set(role.id!, ++existingCount);
      }
    );

    let partRolesToAdd : PartRole[] = [];
    first.forEach( role => {
      let existingAdd = firstIds.get(role.id!);
      let existingRemove = secondIds.get(role.id!);
      if (existingAdd === undefined && existingRemove === undefined)
        return;
      if (existingRemove === undefined && existingAdd !== undefined)
        partRolesToAdd.findIndex((item: PartRole) => item.id === role.id!) === -1 && partRolesToAdd.push(role);
      else if (existingAdd! > existingRemove!)
        partRolesToAdd.findIndex((item: PartRole) => item.id === role.id!) === -1 && partRolesToAdd.push(role);
    });

    let partRolesToRemove : PartRole[] = [];
    second.forEach( role => {
      let existingAdd = firstIds.get(role.id!);
      let existingRemove = secondIds.get(role.id!);
      if (existingAdd === undefined && existingRemove === undefined)
        return;
      if (existingAdd === undefined && existingRemove !== undefined)
        partRolesToRemove.findIndex((item: PartRole) => item.id === role.id!) === -1 && partRolesToRemove.push(role);
      else if (existingAdd! < existingRemove!)
        partRolesToRemove.findIndex((item: PartRole) => item.id === role.id!) === -1 && partRolesToRemove.push(role);
    });

    return {partRolesToAdd, partRolesToRemove};
  }
}