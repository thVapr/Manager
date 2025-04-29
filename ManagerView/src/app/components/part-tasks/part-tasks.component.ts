import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { TaskService } from '../../services/task/task.service';
import { Task } from '../models/Task';
import { AuthService } from '../../services/auth/auth.service';
import { PartService } from '../../services/part/part.service';
import { Status } from '../../status'

@Component({
    selector: 'app-project-tasks',
    templateUrl: './part-tasks.component.html',
    styleUrls: ['./part-tasks.component.scss'],
    standalone: false
})
export class PartTasksComponent implements OnInit {

  searchTaskForm = new FormGroup({
    query: new FormControl('', [Validators.nullValidator]),
  });

  addTaskForm = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.minLength(4)]),
    description: new FormControl('', [Validators.required, Validators.minLength(4)])
  });

  isProjectManager : boolean = false;
  isDepartmentManager : boolean = false;

  todoTasks : Task[] = []
  doingTasks : Task[] = []
  doneTasks : Task[] = []

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
    this.taskService.getFreeTasks().subscribe(tasks => this.todoTasks = tasks);
    this.taskService.getAll().subscribe(tasks => {
      this.doingTasks = tasks.filter(t => t.status == Status.DOING);
      this.doneTasks = tasks.filter(t => t.status == Status.DONE);
    });
  }
 
  addTask() : void {
    let task = new Task;

    task.name = this.addTaskForm.value.name!;
    task.description = this.addTaskForm.value.description!;

    this.taskService.addTask(task).subscribe(() => {
      this.Update();
      this.addTaskForm.reset();
    });
  }

  searchTask() : void {
    const query = this.searchTaskForm.value.query;

    if(query !== null && query !== undefined)
      this.taskService.getTaskByQuery(query).subscribe({
        next: (tasks) => {
          this.todoTasks = tasks.filter(t => t.status == Status.TODO);
          this.doingTasks = tasks.filter(t => t.status == Status.DOING);
          this.doneTasks = tasks.filter(t => t.status == Status.DONE);
        }
      });
  }
}
