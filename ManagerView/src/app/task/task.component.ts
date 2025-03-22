import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { TaskService } from '../services/task/task.service';
import { Task } from '../models/Task';
import { AuthService } from '../services/auth/auth.service';
import { CompanyDepartmentsService } from '../services/company-departments/company-departments.service';
import { ProjectService } from '../services/project/project.service';
import { Status } from '../status'

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
