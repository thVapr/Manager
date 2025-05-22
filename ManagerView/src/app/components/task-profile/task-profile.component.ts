import { Task } from '../models/task';
import { Component } from '@angular/core';
import { Member } from '../models/member';
import { MessageService } from 'primeng/api';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../../services/auth/auth.service';
import { TaskService } from '../../services/task/task.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Location } from '@angular/common';
import { MemberService } from '../../services/member/member.service';
import { TaskActionType, TaskHistory } from '../models/task-history';
import { TaskStatus } from '../models/task-status';
import { PartService } from 'src/app/services/part/part.service';

@Component({
    selector: 'app-task-profile',
    templateUrl: './task-profile.component.html',
    styleUrls: ['./task-profile.component.scss'],
    standalone: false,
    providers: [MessageService],
})
export class TaskProfileComponent {

  TaskActionType = TaskActionType;

  updateTaskForm = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.minLength(4)]),
    description: new FormControl('', [Validators.required, Validators.minLength(4)])
  });

  addMemberForm = new FormGroup({
    member: new FormControl<Member|null>(null, [Validators.required]),
  });

  value: number = 1;
  taskId : string = "";
  taskName : string | undefined = "";
  taskDescription : string | undefined = "";
  status : string | undefined = "";
  taskStatus : number | undefined = 0;
  uploadedFiles: any[] = [];
  taskHistory: TaskHistory[] = [];
  statuses : TaskStatus[] = [];

  availableMembers : Member[] = [];
  isAddMemberFormVisible : boolean = false;

  hasAcceesForEdit : boolean = false;

  creator : Member = {};
  members : Member[] = [];

  constructor(private taskService : TaskService,
              private route : ActivatedRoute,
              private location: Location,
              private memberService : MemberService,
              private authService : AuthService,
              private partService : PartService,
              private messageService: MessageService) {}

  ngOnInit(): void {
    const id : string = this.route.snapshot.params['id'];
    this.taskId = id;
    this.update();
  }

  removeMemberFromTask(member : Member) : void {
    this.taskService.removeMemberFromTask(member.id!, this.taskId).subscribe(
      () => {
        this.update();
      }
    )
  }

  update() : void {
    this.taskService.getTaskById(this.taskId).subscribe((task) => {
      this.taskName = task.name;
      this.taskDescription = task.description;
      this.taskStatus = task.status;
 
      if (task.creatorId !== undefined)
        this.memberService.getMemberById(task.creatorId).subscribe((employee) => {
          this.creator = employee;

          const id = this.authService.getId();

          this.hasAcceesForEdit = id === this.creator.id || this.authService.isAdmin();
          if (this.hasAcceesForEdit) {
            if (this.taskName !== undefined && this.taskDescription !== undefined)
              this.updateTaskForm.setValue({
                name: this.taskName,
                description: this.taskDescription
              });
          }
          this.taskService.getMembersFromTask(task.id!).subscribe((members) => {
            this.members = members;
            this.taskService.getTaskHistory(this.taskId).subscribe({
              next: (history) => {
                this.taskHistory = history.sort((a,b) => {
                  return (new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
                });
                console.log(this.taskHistory);
              }
            });
          });
          this.partService.getPartTaskStatuses().subscribe({
            next: (statuses) => {
              this.statuses = statuses;
            }
          });
        });
    });
  }

  updateTask() : void {
    let task = new Task;

    task.id = this.taskId;
    task.name = this.updateTaskForm.value.name!;
    task.description = this.updateTaskForm.value.description!;
    task.status = this.taskStatus;

    this.taskService.updateTask("", "", task).subscribe(() => {
      this.update();
    });
  }

  onUpload(event:any) {
    for(let file of event.files) {
        this.uploadedFiles.push(file);
    }

    this.messageService.add({severity: 'info', summary: 'File Uploaded', detail: ''});
  }

  showAddMemberToTaskForm() : void {
    this.isAddMemberFormVisible = true;
    this.taskService.getAvailableMembersForTask(this.taskId).subscribe({
      next: (members) => {
        this.availableMembers = members;
        this.addMemberForm.patchValue({
          member: members[0]
        });
      }
    })
  }

  addTaskToMember() : void {
    if (this.addMemberForm.valid) {
      const member = this.addMemberForm.value.member!;
      this.taskService.addTaskToMember(member.id!, this.taskId, 0).subscribe(
        () => {
          this.update();
          this.isAddMemberFormVisible = false;
        }
      );
    }
  }

  removeTask() : void {
    this.taskService.removeTask(this.taskId).subscribe({
      next: () => {
        this.location.back();
      }
    });
  }

  //TODO: вынести в отдельный helper
  getStatusColor(order : number): string {
    const status = this.statuses.find(status => status.order === order)?.globalStatus;
    const colorMap = new Map([
      [0, 'rgba(211, 224, 211, 0.4)'],
      [1, 'rgba(17, 0, 255, 0.4)'],
      [2, 'rgba(252, 255, 173, 0.4)'],
      [3, 'rgba(0, 255, 0, 0.4)'],
      [4, 'rgba(255, 0, 0, 0.4)'],
      [5, 'rgba(252, 255, 173, 0.4)']
    ]);
    return colorMap.get(status!) || 'rgba(51, 170, 51, .1) ';
  }

  getStatusNameByOrder(order : number) {
    return this.statuses.find(status => status.order === order)?.name;
  }
}