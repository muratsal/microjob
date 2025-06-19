import { Routes } from '@angular/router';
import { AdminLayoutComponent } from './shared/components/layouts/admin-layout/admin-layout.component';
import { AuthLayoutComponent } from './shared/components/layouts/auth-layout/auth-layout.component';
import { AuthGuard } from './shared/guards/auth.guard';

export const rootRouterConfig: Routes = [
    {
        path: '',
        redirectTo: 'permission/permission2',
        pathMatch: 'full'
    },
    {
        path: '',
        component: AuthLayoutComponent,
        children: [
            {
                path: 'sessions',
                loadChildren: () => import('./views/sessions/sessions.module').then(m => m.SessionsModule),
                data: { title: 'Session' }
            }
        ]
    },
    {
        path: '',
        component: AdminLayoutComponent,
        canActivate: [AuthGuard],
        children: [
            {
                path: 'others',
                loadChildren: () => import('./views/others/others.module').then(m => m.OthersModule),
                data: { title: 'Others', breadcrumb: 'OTHERS' }
            },
            {
                path: 'search',
                loadChildren: () => import('./views/search-view/search-view.module').then(m => m.SearchViewModule)
            },
            {
                path: 'system',
                loadChildren: () => import('./views/System/system.module').then(m => m.SystemModule),
                data: { title: 'MAINMENU.SYSTEM', breadcrumb: 'MAINMENU.SYSTEM' }
            },

            {
                path: 'employee',
                loadChildren: () => import('./views/Employee/employee.module').then(m => m.EmployeeModule),
                data: { title: 'MAINMENU.EMPLOYEE', breadcrumb: 'MAINMENU.EMPLOYEE' }
            },
            {
                path: 'profile',
                loadChildren: () => import('./views/Profile/profile.module').then(m => m.ProfileModule),
                data: { title: 'MAINMENU.PROFILE', breadcrumb: 'MAINMENU.PROFILE' }
            },
            {
                path: 'inventory',
                loadChildren: () => import('./views/Inventory/inventory.module').then(m => m.InventoryModule),
                data: { title: 'MAINMENU.INVENTORY', breadcrumb: 'MAINMENU.INVENTORY' }
            },
            {
                path: 'permission',
                loadChildren: () => import('./views/permission/permission.module').then(m => m.PermissionModule),
                data: { title: 'MAINMENU.PERMISSION', breadcrumb: 'MAINMENU.PERMISSION' }
            }
           
      


        ]
    },
    {
        path: '**',
        redirectTo: 'sessions/404'
    }
];

