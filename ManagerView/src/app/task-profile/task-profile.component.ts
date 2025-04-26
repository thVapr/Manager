import { Component } from '@angular/core';
import { TaskService } from '../services/task/task.service';
import { ActivatedRoute } from '@angular/router';
import { MemberService } from '../services/member/member.service';
import { Employee } from '../models/Employee';
import { Status } from '../status';
import { Constants } from '../constants';
import { AuthService } from '../services/auth/auth.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Task } from '../models/Task';


@Component({
    selector: 'app-task-profile',
    templateUrl: './task-profile.component.html',
    styleUrls: ['./task-profile.component.scss'],
    standalone: false
})
export class TaskProfileComponent {

  updateTaskForm = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.minLength(4)]),
    description: new FormControl('', [Validators.required, Validators.minLength(4)])
  });


  taskId : string = "";
  taskName : string | undefined = "";
  taskDescription : string | undefined = "";
  status : string | undefined = "";
  taskStatus : number | undefined = 0;

  isAdminOrCreator : boolean = false;

  creator : Employee = new Employee("","","","");
  employee : Employee = new Employee("","","","");

  constructor(private taskService : TaskService,
              private route : ActivatedRoute,
              private memberService : MemberService,
              private authService : AuthService) {}

  ngOnInit(): void {
    const id : string = this.route.snapshot.params['id'];
    this.taskId = id;
    this.Update();
  }

  Update() : void {
    this.getTaskProfile(this.taskId);
  }

  getTaskProfile(id : string) : void {
    this.taskService.getTaskById(id).subscribe((task) => {
      this.taskName = task.name;
      this.taskDescription = task.description;
      this.taskStatus = task.status;

      switch(task.status) {
        case(Status.TODO):
          this.status = Constants.TODO;
          break;
        case(Status.DOING):
          this.status = Constants.DOING;
          break;
        case(Status.DONE):
          this.status = Constants.DONE;
          break;
      }
      
      if (task.creatorId !== undefined)
        this.memberService.getEmployeeById(task.creatorId).subscribe((employee) => {
          this.creator = employee;

          const id = this.authService.getId();

          this.isAdminOrCreator = id === this.creator.id || this.authService.isAdmin();

          if (this.isAdminOrCreator) {
            if (this.taskName !== undefined && this.taskDescription !== undefined)
              this.updateTaskForm.setValue({
                name: this.taskName,
                description: this.taskDescription
              });

          }
        });
      if (task.employeeId !== undefined)
        this.memberService.getEmployeeById(task.employeeId).subscribe((employee) => {
          this.employee = employee;
        });


    });
  }

  updateTask() : void {
    let task = new Task;

    task.id = this.taskId;
    task.name = this.updateTaskForm.value.name!;
    task.description = this.updateTaskForm.value.description!;
    task.status = this.taskStatus;

    this.taskService.updateTask(task).subscribe(() => {
      this.Update();
    });
  }

  returnTask() : void {
    let task = new Task;

    task.id = this.taskId;
    task.status = Status.DOING;

    this.taskService.updateTask(task).subscribe(() => {
      this.Update();
    });
  }
}
