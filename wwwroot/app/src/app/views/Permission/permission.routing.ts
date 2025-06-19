import { Routes } from '@angular/router';
import { PermissionComponent } from './Permission/permission.component';
import { PermissionLogComponent } from './PermissionLog/permissionlog.component';



//@Component_Imports


export const PermissionRoutes: Routes = [
    {
        path: '',
        children: [
            {
                path: 'permission',
                component: PermissionComponent,
                data: { title: 'PERMISSION.PERMISSIONMENU', breadcrumb: 'PERMISSION.PERMISSIONMENU' }
            },
            {
                path: 'permission2',
                component: PermissionComponent,
                data: { title: 'PERMISSION2.PERMISSION2MENU', breadcrumb: 'PERMISSION2.PERMISSION2MENU' }
            },
            {
                path: 'permission3',
                component: PermissionComponent,
                data: { title: 'PERMISSION3.PERMISSION3MENU', breadcrumb: 'PERMISSION3.PERMISSION3MENU' }
            },
            {
                path: 'permissionlog',
                component: PermissionLogComponent,
                data: { title: 'PERMISSIONLOG.PERMISSIONLOGMENU', breadcrumb: 'PERMISSIONLOG.PERMISSIONLOGMENU' }
            }
            //@Component_Routes
        ]
    }
];
