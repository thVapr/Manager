import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { TaskComponent } from './components/task/task.component';
import { PartComponent } from './components/part/part.component';
import { HomeComponent } from './components/home/home.component';
import { TokenInterceptor } from './services/auth/token.intercept';
import { LoginComponent } from './components/login/login.component';
import { MemberComponent } from './components/member/member.component';
import { RegisterComponent } from './components/register/register.component';
import { PartTasksComponent } from './components/part-tasks/part-tasks.component';
import { PartMembersComponent } from './components/part-members/part-members.component';
import { PartProfileComponent } from './components/part-profile/part-profile.component';
import { TaskProfileComponent } from './components/task-profile/task-profile.component';
import { MemberProfileComponent } from './components/member-profile/member-profile.component';

import Lara from '@primeng/themes/lara';
import { providePrimeNG } from 'primeng/config';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';

import { Tag } from 'primeng/tag'
import { Panel } from 'primeng/panel'
import { Avatar } from 'primeng/avatar'
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { EditorModule } from 'primeng/editor';
import { RippleModule } from 'primeng/ripple';
import { TreeModule, Tree } from 'primeng/tree';
import { DragDropModule } from 'primeng/dragdrop'
import { PickListModule } from 'primeng/picklist';
import { DatePickerModule  } from 'primeng/datepicker'

@NgModule({ 
    declarations: [
        AppComponent,
        HomeComponent,
        PartComponent,
        TaskComponent,
        LoginComponent,
        MemberComponent,
        RegisterComponent,
        PartTasksComponent,
        PartMembersComponent,
        PartProfileComponent,
        TaskProfileComponent,
        MemberProfileComponent,
    ],
    exports: [
        LoginComponent,
        RegisterComponent,
    ],
    bootstrap: [AppComponent], 
    imports: [
        Tag,
        Tree,
        Panel,
        Avatar,
        CardModule,
        TreeModule,
        FormsModule,
        EditorModule,
        ButtonModule,
        CommonModule,
        RippleModule,
        BrowserModule,
        DragDropModule,
        PickListModule,
        AppRoutingModule,
        DatePickerModule,
        ReactiveFormsModule,
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
