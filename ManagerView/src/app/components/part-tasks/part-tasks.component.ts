import { Task } from '../models/task';
import { Router } from '@angular/router';
import { PartRole } from '../models/part-role';
import { Component, OnInit } from '@angular/core';
import { TaskStatus } from '../models/task-status';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { TaskService } from '../../services/task/task.service';
import { AuthService } from '../../services/auth/auth.service';
import { PartService } from '../../services/part/part.service';

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
    startDate: new FormControl(null, [Validators.required]),
    deadlineDate: new FormControl(null, [Validators.required]),
    selectedStatusColumns: new FormControl<TaskStatus[]>([],[])
  });

  changeTaskStatusForm = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.minLength(4)]),
    globalStatus: new FormControl(0,[]),
    partRoleId: new FormControl('',[])
  });

  addTaskStatusForm = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.minLength(4)]),
    globalStatus: new FormControl(0,[]),
    partRoleId: new FormControl('',[])
  });

  isLeader : boolean = false;

  statusColumns : TaskStatus[] = [];

  tasks: Task[] = [];
  draggedTask : Task = {};
  draggedStatus : TaskStatus = {};
  isStatusesDraggable : boolean = false;
  isAddTaskFormVisible : boolean = false;
  isChangeStatusFormVisible : boolean = false;
  isAddStatusFormVisible : boolean = false;
  targetStatus : TaskStatus = {};
  partRoles : PartRole[] = [];
  listStatusColumns : TaskStatus[] = [];

  isDraggedBlock: boolean = false;

  constructor(private taskService : TaskService,
    public authService: AuthService,
    public partService: PartService,
    private router: Router) {}

  showAddTaskDialog() : void
  {
    this.listStatusColumns = this.statusColumns.filter(col => col.order !== 111);
    this.addTaskForm.patchValue({
      selectedStatusColumns: this.listStatusColumns
    });
    this.isAddTaskFormVisible = !this.isAddTaskFormVisible;
  }

  getHeaderColor(status: number): string {
    const colorMap = new Map([
      [0, 'rgba(211, 224, 211, 0.4)'],
      [1, 'rgba(17, 0, 255, 0.4)'],
      [2, 'rgba(252, 255, 173, 0.4)'],
      [3, 'rgba(0, 255, 0, 0.4)'],
      [4, 'rgba(255, 0, 0, 0.4)'],
      [5, 'rgba(252, 255, 173, 0.4)']
    ]);
    return colorMap.get(status) || 'rgba(51, 170, 51, .1) ';
  }

  showChangeStatusDialog(status : TaskStatus) : void
  {
    this.targetStatus = status;
    this.changeTaskStatusForm.patchValue({
      name: status.name?.toString(),
      globalStatus: status.globalStatus,
      partRoleId: status.partRoleId || '-1'
    });
    this.isChangeStatusFormVisible = !this.isChangeStatusFormVisible;
  }

  showAddStatusDialog() : void
  {
    this.addTaskStatusForm.patchValue({
      name: "",
      globalStatus: 1,
      partRoleId: '-1'
    });
    this.isAddStatusFormVisible = !this.isAddStatusFormVisible;
  }

  getTaskByStatus(tasks : Task[], status : number)
  {
    return tasks.filter(task => task.status === status);
  }

  statusDragStart(task: TaskStatus) {
    if (this.isDraggedBlock)
      return;
    this.isDraggedBlock = true;
    this.draggedStatus = task;
  }

  statusDrop(index : number) : void {
    if (this.draggedStatus && !this.draggedStatus.isFixed) {
        let colIndex = this.statusColumns.findIndex(col => col.order == index);
        if (this.statusColumns[colIndex].isFixed)
          return;
        let draggedColIndex = this.statusColumns.findIndex(col => col.order == this.draggedStatus.order) 
        let saved = this.statusColumns[colIndex].order;

        this.statusColumns[colIndex].order = this.statusColumns[draggedColIndex].order;
        this.statusColumns[draggedColIndex].order = saved;
      
        this.partService.updatePartStatus(this.statusColumns[colIndex]).subscribe({
          next: () => {
            this.partService.updatePartStatus(this.statusColumns[draggedColIndex]).subscribe({
              next: () => {
                this.update();
                this.draggedStatus = {};
                this.statusColumns.sort((a,b) => a.order! - b.order!);
                this.isDraggedBlock = false;
              }
            });
          }
        });
    }
  }

  statusDragEnd() {
      this.draggedStatus = {};
      this.isDraggedBlock = false;
  }

  dragStart(task: Task) {
    if (this.isDraggedBlock)
      return;
    this.isDraggedBlock = true;
    this.draggedTask = task;
  }

  doubleClick(task: Task) {
    this.router.navigate(['task','about',task.id]);    
  }

  drop(index : number) {

    if (this.draggedTask) {
      var taskIndex = this.findIndex(this.draggedTask);
      this.tasks[taskIndex].status = index;
      this.tasks[taskIndex].partId = this.partService.getPartId()!;
      this.taskService.updateTask(this.tasks[taskIndex]).subscribe(() => {});
      this.draggedTask = {};
      this.isDraggedBlock = false;
    }
  }

  dragEnd() {
      this.draggedTask = {};
      this.isDraggedBlock = false;
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

  private update() : void {
    this.partService.getPartTaskStatuses().subscribe({
      next: (statuses) => {
        this.taskService.getAll().subscribe({next: (tasks) => {
          this.statusColumns = statuses.sort((statusA, statusB) => {
            return statusA.order! - statusB.order!;
          });
          this.tasks = tasks;
          this.tasks.forEach(task => {
            this.taskService.getMembersFromTask(task.id!).subscribe({
              next: (members) => {
                if (members[0] !== undefined)
                {
                  task.memberId = members[0].id!;
                  task.memberName = members[0].firstName;
                  task.memberLastName = members[0].lastName;
                }
  
                this.partService.getRolesById(this.partService.getPartId()!).subscribe({
                  next: (roles) => {
                    this.partRoles = roles;
                }});
            }})
          });
          
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
    task.path = this.addTaskForm.value.selectedStatusColumns!
      .map(value => value.order?.toString())
      .join('-');

    this.taskService.addTask(task).subscribe({
      next: () => {
        this.update();
        this.tasks = [...this.tasks, task];
        this.addTaskForm.reset();
        this.isAddTaskFormVisible = false
      }
    });
  }

  getPartRoleNameById(id : string) : string | undefined {
    return this.partRoles.find(role => role.id === id)?.name;
  }

  changeStatus() : void {
    this.targetStatus.name = this.changeTaskStatusForm.value.name!;
    this.targetStatus.globalStatus = this.changeTaskStatusForm.value.globalStatus!;
    this.targetStatus.partRoleId = this.changeTaskStatusForm.value.partRoleId!;
    this.targetStatus.partId = this.partService.getPartId()!;
    this.targetStatus.order = -1;
    
    this.partService.updatePartStatus(this.targetStatus).subscribe({
      next: () => {
        this.update();
        this.isChangeStatusFormVisible = false
      }
    });
  }

  addStatus() : void {
    if (!this.addTaskStatusForm.valid)
      return;
    
    const statusOrders = this.statusColumns.map(status => {
      if (status.order! < 110)
        return status.order;
      return -1;
    });
    let maxAccessibleOrder = statusOrders.reduce((a,b) => (a! > b!) ? a : b );

    let status = new TaskStatus(
      "",
      this.addTaskStatusForm.value.name!,
      "",
      maxAccessibleOrder! + 1, 
      this.partService.getPartId()!, 
      this.addTaskStatusForm.value.globalStatus!
    );

    status.partRoleId = this.changeTaskStatusForm.value.partRoleId!;
 
    this.partService.addPartStatus(status).subscribe({next: () => {
      this.update();
      this.isAddStatusFormVisible = false;
    }});
  }

  removeStatus(status : TaskStatus) : void
  {
    this.partService.removePartStatus(status.id!).subscribe({
      next: () => {
        this.update();
        this.isChangeStatusFormVisible = false;
      },
      error: () => {
        this.update();
        this.isChangeStatusFormVisible = false;
      }
    })
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
