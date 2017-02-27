/// <reference path="../typings/browser.d.ts"/>

//import {enableProdMode} from '@angular/core';
//import {bootstrap}    from '@angular/platform-browser-dynamic'
//import {HTTP_PROVIDERS} from '@angular/http';

//import {AppServiceTodoList} from './services/app.service.todolist';
//import {TodoListComponent} from './components/todolist/todolist.component';

//enableProdMode();

//import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
//import { AppModule } from './app.module';

//platformBrowserDynamic().bootstrapModule(AppModule);

import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './modules/app.module';

platformBrowserDynamic().bootstrapModule(AppModule)
    .then(success => console.log(`Bootstrap success`))
    .catch(error => console.log(error));

