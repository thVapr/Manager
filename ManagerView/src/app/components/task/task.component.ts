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
  
  isProjectManager : boolean = false;
  isDepartmentManager : boolean = false;

  tasks : Task[] = []
  myTasks : Task[] = []

  constructor(private taskService : TaskService,
    public authService: AuthService,
    public partService: PartService) {}

  ngOnInit(): void {
    this.Update();

    const id = this.authService.getId();
    const partId = this.partService.getPartId();

    if (partId !== null) {
      this.partService.getPartById(partId).subscribe({
        next: (part) => {
          if (part.leaderIds?.includes(id))
            this.isDepartmentManager = true;
        },
        error: () => this.isDepartmentManager = false
      });
    }
  }

  Update() : void {
    this.taskService.getFreeTasks().subscribe(tasks => this.tasks = tasks);
    this.taskService.getTasksByEmployeeId().subscribe(tasks => this.myTasks = tasks.filter(t => t.status != Status.DONE));
  }
 
  ChooseTask(id : string | undefined) : void {
    if(id !== undefined)
      this.taskService.addTaskToEmployee(id).subscribe(() => this.Update());
  }

  completeTask(id : string | undefined) : void {
    let task = new Task();

    task.id = id;
    task.status = Status.DONE;
    
    this.taskService.updateTask(task).subscribe(() => this.Update());
  }
}
