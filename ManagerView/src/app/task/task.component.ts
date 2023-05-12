import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { TaskService } from '../services/task/task.service';
import { Task } from '../models/Task';
import { AuthService } from '../services/auth/auth.service';

@Component({
  selector: 'app-task',
  templateUrl: './task.component.html',
  styleUrls: ['./task.component.scss']
})
export class TaskComponent implements OnInit {
  addTaskForm = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.minLength(4)]),
    description: new FormControl('', [Validators.required, Validators.minLength(4)])
  });

  tasks : Task[] = []
  myTasks : Task[] = []

  constructor(private taskService : TaskService,
    public authService: AuthService) {}

  ngOnInit(): void {
    this.Update();
  }

  Update() : void {
    this.taskService.getFreeTasks().subscribe(tasks => this.tasks = tasks);
    this.taskService.getTasksByEmployeeId().subscribe(tasks => this.myTasks = tasks);
  }
 
  ChooseTask(id : string | undefined) : void {
    if(id !== undefined)
      this.taskService.addTaskToEmployee(id).subscribe(() => this.Update());
  }

  addTask() : void {
    let task = new Task;

    task.name = this.addTaskForm.value.name!;
    task.description = this.addTaskForm.value.description!;

    this.taskService.addTask(task).subscribe(() => this.Update());
  }
}
