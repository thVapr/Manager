import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { TaskService } from '../../services/task/task.service';
import { Task } from '../models/task';
import { AuthService } from '../../services/auth/auth.service';
import { PartService } from '../../services/part/part.service';
import { Status } from '../../status'

@Component({
    selector: 'app-task',
    templateUrl: './task.component.html',
    styleUrls: ['./task.component.scss'],
    standalone: false
})
export class TaskComponent implements OnInit {

  tasks : Task[] = []
  myTasks : Task[] = []

  constructor(private taskService : TaskService,
    public authService: AuthService,
    public partService: PartService) {}

  ngOnInit(): void {
    this.update();
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
        console.log(tasks);
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
