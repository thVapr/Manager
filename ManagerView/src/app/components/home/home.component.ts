import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { PartService } from 'src/app/services/part/part.service';
import { TaskService } from 'src/app/services/task/task.service';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss'],
    standalone: false
})
export class HomeComponent implements OnInit {
    basicData: any;
    basicOptions: any;
    statusCounts: StatusCount[] = [];
    progress: number = 0;

    constructor(private cd: ChangeDetectorRef,
                private partService: PartService,
                private taskService: TaskService) {}

    ngOnInit() : void {
      this.partService.getPartTaskStatuses().subscribe({
        next: (statuses) => {
          this.taskService.getAll().subscribe({
            next: (tasks) => {
              this.statusCounts = statuses.map(status => ({
                statusId: status.id,
                statusName: status.name,
                statusOrder: status.order,
                taskCount: tasks.filter(task => task.status === status.order).length,
                color: this.getHeaderColor(status.globalStatus!)
              })).sort((a,b)=> a.statusOrder! - b.statusOrder!);
              const completedTasks : number = this.statusCounts.find(status => status.statusOrder === 110)?.taskCount!;
              
              this.progress = completedTasks / tasks.length * 100;
              this.initChart();
            }
          });
        }
      });
    }

    private initChart() {
      const documentStyle = getComputedStyle(document.documentElement);
      const textColor = documentStyle.getPropertyValue('--p-text-color');
      const textColorSecondary = documentStyle.getPropertyValue('--p-text-muted-color');
      const surfaceBorder = documentStyle.getPropertyValue('--p-content-border-color');

      this.basicData = {
        labels: [...this.statusCounts.map(status => status.statusName)],
        datasets: [
          {
            label: 'Задачи',
            data: [...this.statusCounts.map(status => status.taskCount)],
            backgroundColor: [...this.statusCounts.map(status => status.color)],
          }
        ],
      };
      this.basicOptions = {
        plugins: {
          legend: {
            labels: {
              color: textColor,
            },
          },
        },
        scales: {
          x: {
            ticks: {
              color: textColorSecondary,
            },
            grid: {
              color: surfaceBorder,
            },
          },
          y: {
            beginAtZero: true,
            ticks: {
              color: textColorSecondary,
            },
            grid: {
              color: surfaceBorder,
            },
          },
        },
      };
      this.cd.markForCheck()
    } 

  getHeaderColor(status: number): string {
    const colorMap = new Map([
      [0, 'rgba(211, 224, 211, 0.4)'],
      [1, 'rgba(17, 0, 255, 0.4)'],
      [2, 'rgba(252, 255, 173, 0.4)'],
      [3, 'rgba(0, 255, 0, 0.4)'],
      [4, 'rgba(255, 0, 0, 0.4)'],
      [5, 'rgba(56, 240, 253, 0.4)']
    ]);
    return colorMap.get(status) || 'rgba(51, 170, 51, .1) ';
  }
}

interface StatusCount {
  statusId?: string;
  statusName?: string;
  statusOrder?: number;
  taskCount?: number;
  color?: string;
}