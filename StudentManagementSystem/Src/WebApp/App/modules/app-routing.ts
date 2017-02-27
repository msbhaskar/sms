import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';


import { SchoolsListComponent } from '../components/marketing/schools-list.component';

const routes: Routes = [
    {
        path: '',
        redirectTo: '/schools',
        pathMatch: 'full'
    },
    {
        path: 'schools',
        component: SchoolsListComponent
    },
    {
        path: 'schools/:id',
        component: SchoolsListComponent
    }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }

export const routedComponents = [SchoolsListComponent];