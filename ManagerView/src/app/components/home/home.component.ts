import { ChangeDetectorRef, Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss'],
    standalone: false
})
export class HomeComponent implements OnInit {
    basicData: any;
    basicOptions: any;
        
    constructor(private cd: ChangeDetectorRef) {}

    ngOnInit() : void {
      this.initChart();
    }

    initChart() {
      const documentStyle = getComputedStyle(document.documentElement);
      const textColor = documentStyle.getPropertyValue('--p-text-color');
      const textColorSecondary = documentStyle.getPropertyValue('--p-text-muted-color');
      const surfaceBorder = documentStyle.getPropertyValue('--p-content-border-color');

      this.basicData = {
        labels: ['Новые', 'В процессе', 'Оцениваются', 'Завершенные', 'Отмененные'],
        datasets: [
          {
            label: 'Задачи',
            data: [11, 5, 3, 10, 1],
            backgroundColor: [
              'rgba(54, 54, 54, 0.15)',
              'rgba(6, 182, 212, 0.2)',
              'rgba(209, 255, 58, 0.2)',
              'rgba(92, 246, 102, 0.2)',
              'rgba(246, 92, 92, 0.2)'
            ],
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
}
