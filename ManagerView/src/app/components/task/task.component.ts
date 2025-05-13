import { Task } from '../models/task';
import { Component, OnInit } from '@angular/core';
import { TaskStatus } from '../models/task-status';
import { TaskService } from '../../services/task/task.service';
import { AuthService } from '../../services/auth/auth.service';
import { PartService } from '../../services/part/part.service';

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
    this.taskService.getFreeTasks().subscribe({
      next: (tasks) => {
        this.tasks = tasks.filter(task => {
          const path = task.path?.split("-")  
            .map(Number);
          if (task.status! >= path![path!.length! - 1])
            return false;
          return true;
        });
      }
    });
    this.taskService.getTasksByMemberId().subscribe({
      next: (tasks) => {
        this.myTasks = tasks.filter(task => {
          if (task.status === undefined || task.path === undefined)
            return false;
          const path = task.path?.split("-")  
            .map(Number).filter(number => !isNaN(number));
          if (path === undefined || path.length === 0)
            return false;
          if (task.status >= path[path.length - 1])
            return false;
          return true;
        });
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
      this.taskService.addTaskToMember(id).subscribe(() => this.update());
  }

  completeTask(id : string | undefined) : void {
    this.taskService.changeTaskStatus(id!)
      .subscribe(() => this.update());
  }
}
