import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';

import '../rxjs-extensions';
import { AppRoutingModule, routedComponents } from './app-routing';


import {AppComponent} from '../components/app.component'
import { SchoolsListComponent } from '../components/marketing/schools-list.component';

import { SchoolsListService } from '../components/marketing/schools-list.service';

@NgModule({
    imports: [
        BrowserModule,
        FormsModule,
        AppRoutingModule,
        HttpModule],
declarations: [
        AppComponent,
        SchoolsListComponent,
        routedComponents
    ],
    providers: [
        SchoolsListService
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }