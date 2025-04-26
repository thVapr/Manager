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
import { PartComponent } from './part/part.component';
import { PartLinksComponent } from './part-links/part-links.component';
import { MemberComponent } from './member/member.component';
import { PartMembersComponent } from './part-members/part-members.component';
import { TaskComponent } from './task/task.component';
import { PartProfileComponent } from './part-profile/part-profile.component';
import { PartTasksComponent } from './part-tasks/part-tasks.component';
import { TaskProfileComponent } from './task-profile/task-profile.component';
import { MemberProfileComponent } from './member-profile/member-profile.component';

@NgModule({ declarations: [
        AppComponent,
        HomeComponent,
        LoginComponent,
        RegisterComponent,
        PartComponent,
        PartLinksComponent,
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
