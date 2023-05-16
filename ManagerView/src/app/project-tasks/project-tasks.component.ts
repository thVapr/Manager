import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { TaskService } from '../services/task/task.service';
import { Task } from '../models/Task';
import { AuthService } from '../services/auth/auth.service';
import { CompanyDepartmentsService } from '../services/company-departments/company-departments.service';
import { ProjectService } from '../services/project/project.service';
import { Status } from '../status'

@Component({
  selector: 'app-project-tasks',
  templateUrl: './project-tasks.component.html',
  styleUrls: ['./project-tasks.component.scss']
})
export class ProjectTasksComponent implements OnInit {

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
    public departmentService: CompanyDepartmentsService,
    public projectService: ProjectService) {}

  ngOnInit(): void {
    this.Update();

    const id = this.authService.getId();
    const departmentId = this.departmentService.getDepartmentId();

    if (departmentId !== null) {
      this.departmentService.getDepartment(departmentId).subscribe({
        next: (department) => {
          if (department.managerId !== null && department.managerId !== undefined && department.managerId == id)
            this.isDepartmentManager = true;
        },
        error: () => this.isDepartmentManager = false
      });
    }

    const projectId = this.projectService.getProjectId();

    if (projectId !== null) {
      this.projectService.getProjectById(projectId).subscribe({
        next: (project) => {
          if (project.managerId !== null && project.managerId !== undefined && project.managerId == id)
            this.isProjectManager = true;
        },
        error: () => this.isProjectManager = false
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
