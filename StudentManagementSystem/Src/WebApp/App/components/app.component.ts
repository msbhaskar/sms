import { Component } from '@angular/core';

@Component({
    //moduleId: module.id,
    selector: 'my-app',
    template: `
    <h1>{{title}}</h1>
    <div class="header-bar"></div>
    <nav>
      <a routerLink="/schools" routerLinkActive="active">Schools</a>
      <a routerLink="/curriculum" routerLinkActive="active">Curriculum</a>
    </nav>
    <router-outlet></router-outlet>
  `,
    styleUrls: ['app.component.css']
})
export class AppComponent {
    title = 'Student Management System';
}