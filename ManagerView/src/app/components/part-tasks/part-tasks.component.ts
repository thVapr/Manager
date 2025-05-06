import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { TaskService } from '../../services/task/task.service';
import { Task } from '../models/task';
import { AuthService } from '../../services/auth/auth.service';
import { PartService } from '../../services/part/part.service';
import { Router } from '@angular/router';
import { TaskStatus } from '../models/task-status';

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
    description: new FormControl('', [Validators.required, Validators.minLength(4)]),
    startDate: new FormControl('', [Validators.required]),
    deadlineDate: new FormControl('', [Validators.required])
  });

  isLeader : boolean = false;

  statusColumns : TaskStatus[] = [];

  tasks: Task[] = [];
  draggedTask : Task = {};

  constructor(private taskService : TaskService,
    public authService: AuthService,
    public partService: PartService,
    private router: Router) {}

  draggedOver: string | null = null;
  
  getTaskByStatus(tasks : Task[], status : number)
  {
    return tasks.filter(task => task.status === status);
  }

  dragStart(task: Task) {
    this.draggedTask = task;
  }

  doubleClick(task: Task) {
    this.router.navigate(['task','about',task.id]);    
    console.log(task.name + " was double clicked with id " + task.id)
  }

  drop(index : number) {
      if (this.draggedTask) {
          var taskIndex = this.findIndex(this.draggedTask);
          this.tasks[taskIndex].status = index;
          this.taskService.updateTask(this.tasks[taskIndex]).subscribe(() => {});
          this.draggedTask = {};
      }
  }

  dragEnd() {
      this.draggedTask = {};
  }

  findIndex(task: Task) {
      let index = -1;
      for (let i = 0; i < (this.tasks as Task[]).length; i++) {
          if (task.id === (this.tasks as Task[])[i].id) {
              index = i;
              break;
          }
      }
      return index;
  }

  ngOnInit(): void {
    this.update();

    const id = this.authService.getId();
    const partId = this.partService.getPartId();

    if (partId !== null) {
      this.partService.hasPrivileges(id, partId, 5).subscribe({
        next: (response) => this.isLeader = response,
        error: () => this.isLeader = false
      });
    }
  }

  update() : void {
    this.partService.getPartTaskStatuses().subscribe({
      next: (statuses) => {
        this.taskService.getAll().subscribe({next: (tasks) => {
          this.statusColumns = statuses;
          this.tasks = tasks;
        }});
      }
    });
  }
 
  addTask() : void {
    let task = new Task;

    task.name = this.addTaskForm.value.name!;
    task.description = this.addTaskForm.value.description!;
    task.startTime = new Date(this.addTaskForm.value.startDate!);
    task.deadline = new Date(this.addTaskForm.value.deadlineDate!);

    this.taskService.addTask(task).subscribe({
      next: () => {
        this.update();
        this.tasks = [...this.tasks, task];
        this.addTaskForm.reset();
        }
    });
  }

  searchTask() : void {
    const query = this.searchTaskForm.value.query;

    if(query !== null && query !== undefined && query !== "")
    {
      this.taskService.getTaskByQuery(query).subscribe({
        next: (tasks) => {
          this.tasks = tasks;
        }
      });
    }
    else
    {
      this.update();
    }
  }
}
