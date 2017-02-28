import { Component } from '@angular/core';

@Component({
    //moduleId: module.id,
    selector: 'app-container',
    template: `
    <nav>
      <a routerLink="/marketing/schools" routerLinkActive="active">Schools</a>
      <a routerLink="/marketing/curriculum" routerLinkActive="active">Curriculum</a>
    </nav>
    <router-outlet></router-outlet>
  `
    //,//styleUrls: ['app.component.css']
})
export class AppComponent {
    title = 'Student Management System';
}