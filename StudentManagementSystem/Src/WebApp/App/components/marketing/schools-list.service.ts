﻿import {Injectable} from '@angular/core';
import { Headers, Http, Response } from '@angular/http';
import 'rxjs/add/operator/toPromise';

import SchoolViewModel = StudentManagementSystem.Data.ViewModels.Schools.SchoolViewModel;

@Injectable()
export class SchoolsListService {

    private getSchoolsUrl = 'api/schools';

    constructor(private http: Http) { }

    getSchools(): Promise<SchoolViewModel[]> {
        return this.http
            .get(this.getSchoolsUrl)
            .toPromise()
            .then(this.successCallback)
            .catch(this.handleError);
    }

    private successCallback(response : any) {
        return response.json() as SchoolViewModel[];
    }
    private handleError(error: any): Promise<any> {
        console.error('An error occurred', error);
        return Promise.reject(error.message || error);
    }
}