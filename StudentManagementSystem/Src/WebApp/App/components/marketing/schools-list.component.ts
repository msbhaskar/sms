/// <reference path="../../../Scripts/TypeLite.Net4.d.ts"/>
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import {SchoolsListService} from "./schools-list.service";

import SchoolViewModel = StudentManagementSystem.Data.ViewModels.Schools.SchoolViewModel;

@Component({
    //moduleId: module.id,
    selector: 'schools-list',
    template: `
    <h1>{{title}}</h1>
    <ul class="schools">
  <li *ngFor="let s of schools" (click)="onSelect(school)" [class.selected]="school === selected">
    <span class="school-element">
      <span class="badge">{{school.id}}</span> {{school.name}}
    </span>
  </li>
</ul>

<div class="error" *ngIf="error">{{error}}</div>
`
})
export class SchoolsListComponent implements OnInit {

    title = 'List of Schools';
    schools: SchoolViewModel[];
    selected: SchoolViewModel;

    error: any;

    constructor(
        private router: Router,
        private service: SchoolsListService) { }

    getSchools(): void {
        this.service
            .getSchools()
            .then(schools => this.schools = schools)
            .catch(error => this.error = error);
    }

    onSelect(sel: SchoolViewModel): void {
        this.selected = sel;
    }

    ngOnInit(): void {
        this.getSchools();
    }

}
