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

const routes: Routes = [
  {path: 'register', component: RegisterComponent},
  {path: 'login', component: LoginComponent},
  {path: 'home', component: HomeComponent},
  {path: 'employee', component: EmployeeComponent},
  {path: 'company', component: CompanyComponent},
  {path: 'company/about/:id', component: CompanyProfileComponent},  
  {path: 'company/departments', component: CompanyDepartmentsComponent},
  {path: 'department/employees', component: DepartmentEmployeesComponent},
  {path: 'department/about/:id', component: DepartmentProfileComponent},
  {path: 'project', component: ProjectComponent},
  {path: 'project/about/:id', component: ProjectProfileComponent},
  {path: 'project/tasks', component: TaskComponent},
  {path: 'project/employees', component: ProjectEmployeesComponent},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
