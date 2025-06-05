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
import { RegisterComponent } from './components/register/register.component';
import { PartTasksComponent } from './components/part-tasks/part-tasks.component';
import { PartControlComponent } from './components/part-control/part-control.component';
import { PartProfileComponent } from './components/part-profile/part-profile.component';
import { TaskProfileComponent } from './components/task-profile/task-profile.component';
import { MemberProfileComponent } from './components/member-profile/member-profile.component';

import Lara from '@primeng/themes/Lara';
import { providePrimeNG } from 'primeng/config';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';

import { Tag } from 'primeng/tag'
import { Menu } from 'primeng/menu'
import { Panel } from 'primeng/panel'
import { Avatar } from 'primeng/avatar'
import { CardModule } from 'primeng/card';
import { TabsModule } from 'primeng/tabs';
import { ChipsModule } from 'primeng/chips';
import { ToastModule } from 'primeng/toast';
import { ChartModule } from 'primeng/chart';
import { DropdownModule } from 'primeng/dropdown';
import { ButtonModule } from 'primeng/button';
import { EditorModule } from 'primeng/editor';
import { RippleModule } from 'primeng/ripple';
import { DialogModule } from 'primeng/dialog';
import { definePreset } from '@primeng/themes';
import { FloatLabel } from 'primeng/floatlabel'
import { ListboxModule } from 'primeng/listbox';
import { TreeModule, Tree } from 'primeng/tree';
import { InplaceModule } from 'primeng/inplace';
import { MenubarModule } from 'primeng/menubar';
import { DragDropModule } from 'primeng/dragdrop'
import { PickListModule } from 'primeng/picklist';
import { DatePickerModule  } from 'primeng/datepicker'
import { FileUploadModule } from 'primeng/fileupload';
import { InputNumberModule } from 'primeng/inputnumber'
import { ProgressBarModule } from 'primeng/progressbar';


const MyPreset = definePreset(Lara, {
  primitive: {
    green: {
      500: '#2f4f4f',
    },
  },
  semantic: {
    primary: {
      500: '{green.500}',
    },
  },
});

@NgModule({ 
    declarations: [
        AppComponent,
        HomeComponent,
        PartComponent,
        TaskComponent,
        LoginComponent,
        RegisterComponent,
        PartTasksComponent,
        PartControlComponent,
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
        TabsModule,
        ChartModule,
        ChipsModule,
        FormsModule,
        ToastModule,
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
        DropdownModule,
        AppRoutingModule,
        DatePickerModule,
        FileUploadModule,
        ProgressBarModule,
        InputNumberModule,
        ReactiveFormsModule,
    ],
    providers: [
        provideAnimationsAsync(),
        providePrimeNG({ theme : { 
            preset : MyPreset,
            options: {
                darkModeSelector: false || 'none',
            },
        }}),
        {
            provide: HTTP_INTERCEPTORS,
            useClass: TokenInterceptor,
            multi: true
        },
        provideHttpClient(withInterceptorsFromDi())
    ] })
export class AppModule {}
