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
import { Menu } from 'primeng/menu'
import { Panel } from 'primeng/panel'
import { Avatar } from 'primeng/avatar'
import { CardModule } from 'primeng/card';
import { ChipsModule } from 'primeng/chips';
import { ButtonModule } from 'primeng/button';
import { FloatLabel } from 'primeng/floatlabel'
import { EditorModule } from 'primeng/editor';
import { RippleModule } from 'primeng/ripple';
import { DialogModule } from 'primeng/dialog';
import { ListboxModule } from 'primeng/listbox';
import { TreeModule, Tree } from 'primeng/tree';
import { InplaceModule } from 'primeng/inplace';
import { MenubarModule } from 'primeng/menubar';
import { DragDropModule } from 'primeng/dragdrop'
import { PickListModule } from 'primeng/picklist';
import { DatePickerModule  } from 'primeng/datepicker'
import { InputNumberModule } from 'primeng/inputnumber'

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
        Menu,
        Panel,
        Avatar,
        FloatLabel,
        CardModule,
        TreeModule,
        ChipsModule,
        FormsModule,
        EditorModule,
        ButtonModule,
        CommonModule,
        RippleModule,
        DialogModule,
        InplaceModule,
        ListboxModule,
        BrowserModule,
        MenubarModule,
        DragDropModule,
        PickListModule,
        AppRoutingModule,
        DatePickerModule,
        InputNumberModule,
        ReactiveFormsModule,
    ],
    providers: [
        provideAnimationsAsync(),
        providePrimeNG({ theme : { 
            preset : Lara,
            options: {
                darkModeSelector: false || 'none',
                presetOptions: {
                    primary: '#3B82F6',
                    surface: '#ffffff',
                    borderRadius: 12
                }
            }}}),
        {
            provide: HTTP_INTERCEPTORS,
            useClass: TokenInterceptor,
            multi: true
        },
        provideHttpClient(withInterceptorsFromDi())
    ] })
export class AppModule {}
