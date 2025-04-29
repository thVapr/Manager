import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegisterComponent } from './components/register/register.component';
import { LoginComponent } from './components/login/login.component';
import { HomeComponent } from './components/home/home.component';
import { MemberComponent } from './components/member/member.component';
import { PartComponent } from './components/part/part.component';
import { PartMembersComponent } from './components/part-members/part-members.component';
import { TaskComponent } from './components/task/task.component';
import { PartProfileComponent } from './components/part-profile/part-profile.component';
import { PartTasksComponent } from './components/part-tasks/part-tasks.component';
import { TaskProfileComponent } from './components/task-profile/task-profile.component';
import { AuthGuard } from './guards/auth-guard.guard';
import { AdminGuard } from './guards/admin-guard.guard';
import { PartGuard } from './guards/part-guard.guard';
import { MemberProfileComponent } from './components/member-profile/member-profile.component';

const routes: Routes = [
  {path: 'register', component: RegisterComponent},
  {path: 'login', component: LoginComponent},
  {path: 'home', component: HomeComponent},
  {path: 'employee', component: MemberComponent, canActivate: [AuthGuard]},
  {path: 'employee/tasks', component: TaskComponent, canActivate: [AuthGuard]},
  {path: 'company', component: PartComponent, canActivate: [AuthGuard,AdminGuard]},
  {path: 'company/about/:id', component: PartProfileComponent, canActivate: [AuthGuard]},  
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
