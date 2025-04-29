import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { TokenInterceptor } from './services/auth/token.intercept';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './components/home/home.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { PartComponent } from './components/part/part.component';
import { MemberComponent } from './components/member/member.component';
import { PartMembersComponent } from './components/part-members/part-members.component';
import { TaskComponent } from './components/task/task.component';
import { PartProfileComponent } from './components/part-profile/part-profile.component';
import { PartTasksComponent } from './components/part-tasks/part-tasks.component';
import { TaskProfileComponent } from './components/task-profile/task-profile.component';
import { MemberProfileComponent } from './components/member-profile/member-profile.component';

import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { providePrimeNG } from 'primeng/config';
import Lara from '@primeng/themes/lara';

import { TreeModule, Tree } from 'primeng/tree';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';

@NgModule({ 
    declarations: [
        AppComponent,
        HomeComponent,
        LoginComponent,
        RegisterComponent,
        PartComponent,
        MemberComponent,
        PartMembersComponent,
        TaskComponent,
        PartProfileComponent,
        TaskProfileComponent,
        PartTasksComponent,
        MemberProfileComponent,
    ],
    exports: [
        LoginComponent,
        RegisterComponent
    ],
    bootstrap: [AppComponent], 
    imports: [BrowserModule,
        AppRoutingModule,
        FormsModule,
        CommonModule,
        ReactiveFormsModule,
        TreeModule,
        CardModule,
        ButtonModule,
        Tree
    ],
    providers: [
        provideAnimationsAsync(),
        providePrimeNG({ theme : { 
            preset : Lara,
            options: {
                darkModeSelector: false || 'none'
            }}}),
        {
            provide: HTTP_INTERCEPTORS,
            useClass: TokenInterceptor,
            multi: true
        },
        provideHttpClient(withInterceptorsFromDi())
    ] })
export class AppModule {}
