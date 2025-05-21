import { Task } from '../models/task';
import { Component, OnInit } from '@angular/core';
import { TaskStatus } from '../models/task-status';
import { TaskService } from '../../services/task/task.service';
import { AuthService } from '../../services/auth/auth.service';
import { PartService } from '../../services/part/part.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
    selector: 'app-task',
    templateUrl: './task.component.html',
    styleUrls: ['./task.component.scss'],
    standalone: false
})
export class TaskComponent implements OnInit {

  tasks : Task[] = [];
  myTasks : Task[] = [];
  statuses : TaskStatus[] = [];
  isTaskChangeStatusFormVisible : boolean = false;
  private targetTask : Task = {};
  statusPrev : TaskStatus | undefined = undefined;
  statusFrom : TaskStatus | undefined = {};
  statusTo : TaskStatus | undefined = {};

  changeTaskStatusForm = new FormGroup({
    name: new FormControl('', []),
    description: new FormControl('', []),
    forward: new FormControl(true, [])
  });

  textMapColor : Map<number, string[]> = new Map([
    [0, ["text-primary", "Обычный"]],
    [1, ["text-warning", "Средний"]],
    [2, ["text-danger", "Высокий"]],
    [3, ["text-danger bg-dark", "Экстремальный"]],
  ]);

  constructor(private taskService : TaskService,
    public authService: AuthService,
    public partService: PartService) {}

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

  ngOnInit(): void {
    this.update();
  }

  getStatusNameByOrder(order : number) {
    return this.statuses.find(status => status.order === order)?.name;
  }

  private update() : void {
    this.taskService.getAvailableTasks().subscribe({
      next: (tasks) => {
        this.tasks = tasks.filter(task => {
          const path = task.path?.split("-")  
            .map(Number);
          if (task.status! >= path![path!.length! - 1])
            return false;
          return true;
        }).sort((a,b) => (b.level!-a.level!));
      }
    });
    this.taskService.getTasksByMemberId().subscribe({
      next: (tasks) => {
        const partId = this.partService.getPartId();
        this.myTasks = tasks.filter(task => {
          if (task.status === undefined || task.path === undefined)
            return false;
          const path = task.path?.split("-")  
            .map(Number).filter(number => !isNaN(number));
          if (path === undefined || path.length === 0)
            return false;
          if (task.status >= path[path.length - 1])
            return false;
          if (partId !== task.partId)
            return false;
          return true;
        }).sort((a,b) => (b.level!-a.level!));
        this.partService.getPartTaskStatuses().subscribe({
          next: (statuses) => {
            this.statuses = statuses;
          }
        });
      }
    });
  }
 
  chooseTask(id : string | undefined) : void {
    if(id !== undefined)
      this.taskService.addTaskToCurrentMember(id).subscribe(() => this.update());
  }

  changeTaskStatusFormInitialise(task : Task | undefined) : void {
    this.targetTask = task!;
    
    const parsedPath = this.targetTask.path?.split('-');
    const pathIndex = parsedPath!.findIndex(node => node === task!.status?.toString());
    if (pathIndex != 0)
      this.statusPrev = this.statuses.find(status => status.order?.toString() === parsedPath![pathIndex - 1]);
    this.statusFrom = this.statuses.find(status => status.order?.toString() === parsedPath![pathIndex]);
    this.statusTo = this.statuses.find(status => status.order?.toString() === parsedPath![pathIndex + 1]);

    this.isTaskChangeStatusFormVisible = !this.isTaskChangeStatusFormVisible;
  }

  changeTaskStatus() : void {
    const name = this.changeTaskStatusForm.value.name;
    const description = this.changeTaskStatusForm.value.description;
    const forward = this.changeTaskStatusForm.value.forward;

    this.taskService.changeTaskStatus(name!, description!, this.targetTask!.id!, forward!)
      .subscribe({
        next: () => {
          this.update();
          this.canceledStatusChange();
          this.changeTaskStatusForm.patchValue({
            name: '',
            description: '',
            forward: true
          });
        }
      });
  }

  toggleDirection(): void {
    if (this.statusPrev === undefined)
      return;
    const current = this.changeTaskStatusForm.get('forward')?.value;
    this.changeTaskStatusForm.get('forward')?.setValue(!current);
  }

  canceledStatusChange() : void {
    this.statusPrev = undefined;
    this.statusFrom = {};
    this.statusTo = {};
    this.targetTask = {};
    this.isTaskChangeStatusFormVisible = false;
  }
}