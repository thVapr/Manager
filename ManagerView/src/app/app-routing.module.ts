import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { OwnerGuard } from './guards/owner.guard';
import { AuthGuard } from './guards/auth-guard.guard';
import { PartGuard } from './guards/part-guard.guard';
import { AdminGuard } from './guards/admin-guard.guard';
import { HomeComponent } from './components/home/home.component';
import { PartComponent } from './components/part/part.component';
import { TaskComponent } from './components/task/task.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { PartTasksComponent } from './components/part-tasks/part-tasks.component';
import { PartProfileComponent } from './components/part-profile/part-profile.component';
import { TaskProfileComponent } from './components/task-profile/task-profile.component';
import { PartControlComponent } from './components/part-control/part-control.component';
import { MemberProfileComponent } from './components/member-profile/member-profile.component';

const routes: Routes = [
  {path: 'home', component: HomeComponent, canActivate: [AuthGuard]},
  {path: 'login', component: LoginComponent},
  {path: 'register', component: RegisterComponent},
  {path: 'parts', component: PartComponent, canActivate: [AuthGuard]},
  {path: 'member/tasks', component: TaskComponent, canActivate: [AuthGuard]},
  {path: 'part/tasks', component: PartTasksComponent, canActivate: [AuthGuard]},
  {path: 'task/about/:id', component: TaskProfileComponent, canActivate: [AuthGuard]},
  {path: 'member/about/:id', component: MemberProfileComponent, canActivate: [AuthGuard]},
  {path: 'part/members', component: PartControlComponent, canActivate: [AuthGuard, PartGuard]},
  {path: 'part/about/:id', component: PartProfileComponent, canActivate: [AuthGuard, PartGuard]},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
