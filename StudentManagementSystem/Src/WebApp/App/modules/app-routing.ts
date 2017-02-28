import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';


import { SchoolsListComponent } from '../components/marketing/schools-list.component';

const routes: Routes = [
    {
        path: '',
        redirectTo: 'marketing/schools',
        pathMatch: 'full'
    },
    {
        path: 'marketing/schools',
        component: SchoolsListComponent
    },
    {
        path: 'marketing/schools/:id',
        component: SchoolsListComponent
    }
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule]
})
export class AppRoutingModule { }

export const routedComponents = [SchoolsListComponent];