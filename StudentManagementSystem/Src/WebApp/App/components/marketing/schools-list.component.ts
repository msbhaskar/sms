/// <reference path="../../../Scripts/TypeLite.Net4.d.ts"/>
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

import {SchoolsListService} from "./schools-list.service";

import SchoolViewModel = StudentManagementSystem.Data.ViewModels.Schools.SchoolViewModel;

@Component({
    //moduleId: module.id,
    selector: 'schools-list',
    templateUrl: '/app/components/marketing/schools-list.view.html'
    // template: ``
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
            .then(schools => {
                this.schools = schools;
                console.log("Count:" + schools.length);
            })
            .catch(error => this.error = error);
    }

    onSelect(sel: SchoolViewModel): void {
        this.selected = sel;
    }

    ngOnInit(): void {
        this.getSchools();
    }

}
