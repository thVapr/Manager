import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegisterComponent } from './register/register.component';
import { LoginComponent } from './login/login.component';
import { HomeComponent } from './home/home/home.component';
import { PartLinksComponent } from './part-links/part-links.component';
import { MemberComponent } from './member/member.component';
import { PartComponent } from './part/part.component';
import { PartMembersComponent } from './part-members/part-members.component';
import { TaskComponent } from './task/task.component';
import { PartProfileComponent } from './part-profile/part-profile.component';
import { PartTasksComponent } from './part-tasks/part-tasks.component';
import { TaskProfileComponent } from './task-profile/task-profile.component';
import { AuthGuard } from './guards/auth-guard.guard';
import { AdminGuard } from './guards/admin-guard.guard';
import { PartGuard } from './guards/part-guard.guard';
import { MemberProfileComponent } from './member-profile/member-profile.component';

const routes: Routes = [
  {path: 'register', component: RegisterComponent},
  {path: 'login', component: LoginComponent},
  {path: 'home', component: HomeComponent},
  {path: 'employee', component: MemberComponent, canActivate: [AuthGuard]},
  {path: 'employee/tasks', component: TaskComponent, canActivate: [AuthGuard]},
  {path: 'company', component: PartComponent, canActivate: [AuthGuard,AdminGuard]},
  {path: 'company/about/:id', component: PartProfileComponent, canActivate: [AuthGuard]},  
  {path: 'company/departments', component: PartLinksComponent, canActivate: [AuthGuard]},
  {path: 'department/employees', component: PartMembersComponent, canActivate: [AuthGuard, PartGuard]},
  {path: 'department/about/:id', component: PartProfileComponent, canActivate: [AuthGuard]},
  {path: 'project', component: PartComponent, canActivate: [AuthGuard, PartGuard]},
  {path: 'project/about/:id', component: PartProfileComponent, canActivate: [AuthGuard]},
  {path: 'project/employees', component: PartMembersComponent, canActivate: [AuthGuard, PartGuard]},
  {path: 'project/tasks', component: PartTasksComponent, canActivate: [AuthGuard]},
  {path: 'task/about/:id', component: TaskProfileComponent, canActivate: [AuthGuard]},
  {path: 'employee/about/:id', component: MemberProfileComponent, canActivate: [AuthGuard]},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
