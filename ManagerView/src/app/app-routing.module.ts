import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegisterComponent } from './register/register.component';
import { LoginComponent } from './login/login.component';
import { HomeComponent } from './home/home/home.component';
import { CompanyComponent } from './company/company.component';
import { CompanyDepartmentsComponent } from './company-departments/company-departments.component';
import { EmployeeComponent } from './employee/employee.component';
import { ProjectComponent } from './project/project.component';
import { DepartmentEmployeesComponent } from './department-employees/department-employees.component';
import { ProjectEmployeesComponent } from './project-employees/project-employees.component';
import { TaskComponent } from './task/task.component';
import { ProjectProfileComponent } from './project-profile/project-profile.component';
import { CompanyProfileComponent } from './company-profile/company-profile.component';
import { DepartmentProfileComponent } from './department-profile/department-profile.component';
import { ProjectTasksComponent } from './project-tasks/project-tasks.component';
import { TaskProfileComponent } from './task-profile/task-profile.component';
import { AuthGuard } from './guards/auth-guard.guard';
import { DepartmentGuard } from './guards/department-guard.guard';
import { AdminGuard } from './guards/admin-guard.guard';
import { ProjectGuard } from './guards/project-guard.guard';
import { EmployeeProfileComponent } from './employee-profile/employee-profile.component';

const routes: Routes = [
  {path: 'register', component: RegisterComponent},
  {path: 'login', component: LoginComponent},
  {path: 'home', component: HomeComponent},
  {path: 'employee', component: EmployeeComponent, canActivate: [AuthGuard]},
  {path: 'employee/tasks', component: TaskComponent, canActivate: [AuthGuard]},
  {path: 'company', component: CompanyComponent, canActivate: [AuthGuard,AdminGuard]},
  {path: 'company/about/:id', component: CompanyProfileComponent, canActivate: [AuthGuard]},  
  {path: 'company/departments', component: CompanyDepartmentsComponent, canActivate: [AuthGuard]},
  {path: 'department/employees', component: DepartmentEmployeesComponent, canActivate: [AuthGuard,DepartmentGuard]},
  {path: 'department/about/:id', component: DepartmentProfileComponent, canActivate: [AuthGuard]},
  {path: 'project', component: ProjectComponent, canActivate: [AuthGuard, DepartmentGuard]},
  {path: 'project/about/:id', component: ProjectProfileComponent, canActivate: [AuthGuard]},
  {path: 'project/employees', component: ProjectEmployeesComponent, canActivate: [AuthGuard, ProjectGuard]},
  {path: 'project/tasks', component: ProjectTasksComponent, canActivate: [AuthGuard]},
  {path: 'task/about/:id', component: TaskProfileComponent, canActivate: [AuthGuard]},
  {path: 'employee/about/:id', component: EmployeeProfileComponent, canActivate: [AuthGuard]},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
