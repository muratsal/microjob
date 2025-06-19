import { Routes } from '@angular/router';

import {  EmployeeComponent } from './Employee/employee.component';


//@Component_Imports


export const EmployeeRoutes: Routes = [
    {
        path: '',
        children: [
            {
                path: 'employee',
                component: EmployeeComponent,
                data: { title: 'EMPLOYEE.EMPLOYEEMENU', breadcrumb: 'EMPLOYEE.EMPLOYEEMENU' }
            }
         
            //@Component_Routes
        ]
    }
];
