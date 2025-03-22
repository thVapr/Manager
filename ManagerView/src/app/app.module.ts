import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { TokenInterceptor } from './services/auth/token.intercept';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home/home.component';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { CompanyComponent } from './company/company.component';
import { CompanyDepartmentsComponent } from './company-departments/company-departments.component';
import { EmployeeComponent } from './employee/employee.component';
import { ProjectComponent } from './project/project.component';
import { ProjectEmployeesComponent } from './project-employees/project-employees.component';
import { DepartmentEmployeesComponent } from './department-employees/department-employees.component';
import { TaskComponent } from './task/task.component';
import { CompanyProfileComponent } from './company-profile/company-profile.component';
import { DepartmentProfileComponent } from './department-profile/department-profile.component';
import { ProjectProfileComponent } from './project-profile/project-profile.component';
import { ProjectTasksComponent } from './project-tasks/project-tasks.component';
import { TaskProfileComponent } from './task-profile/task-profile.component';
import { EmployeeProfileComponent } from './employee-profile/employee-profile.component';

@NgModule({ declarations: [
        AppComponent,
        HomeComponent,
        LoginComponent,
        RegisterComponent,
        CompanyComponent,
        CompanyDepartmentsComponent,
        EmployeeComponent,
        ProjectComponent,
        ProjectEmployeesComponent,
        DepartmentEmployeesComponent,
        TaskComponent,
        CompanyProfileComponent,
        DepartmentProfileComponent,
        ProjectProfileComponent,
        ProjectTasksComponent,
        TaskProfileComponent,
        EmployeeProfileComponent,
    ],
    exports: [
        LoginComponent,
        RegisterComponent
    ],
    bootstrap: [AppComponent], imports: [BrowserModule,
        AppRoutingModule,
        FormsModule,
        CommonModule,
        ReactiveFormsModule], providers: [
        {
            provide: HTTP_INTERCEPTORS,
            useClass: TokenInterceptor,
            multi: true
        },
        provideHttpClient(withInterceptorsFromDi())
    ] })
export class AppModule {}
