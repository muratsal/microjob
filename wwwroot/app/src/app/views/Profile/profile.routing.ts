import { Routes } from '@angular/router';
import { OverviewComponent } from './Overview/overview.component';


//@Component_Imports


export const ProfileRoutes: Routes = [
    {
        path: '',
        children: [
            {
                path: 'overview',
                component: OverviewComponent,
                data: { title: 'PROFILE.OVERVIEWMENU', breadcrumb: 'PROFILE.OVERVIEWMENU' }
            }
            //@Component_Routes
        ]
    }
];
