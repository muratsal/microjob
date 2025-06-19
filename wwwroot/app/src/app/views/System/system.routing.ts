import { Routes } from '@angular/router';

import { UserComponent } from './User/user.component';
import { RoleComponent } from './Role/role.component';
import { AuthComponent } from './Auth/auth.component';
import { UserRoleComponent } from './UserRole/userrole.component';
import { RoleAuthComponent } from './RoleAuth/roleauth.component';
import { MenuComponent } from './Menu/menu.component';
import { EmailComponent } from './Email/email.component';
import { EmailConfigComponent } from './EmailConfig/emailconfig.component';
import { LogComponent } from './Log/log.component';
import { EmailAttachmentComponent } from './EmailAttachment/emailattachment.component';
import { ParameterComponent } from './Parameter/parameter.component';
import { CounterComponent } from './Counter/counter.component';
import { FileComponent } from './File/file.component';
import { UserPageConfigurationComponent } from './UserPageConfiguration/userpageconfiguration.component';
import { CompanyComponent } from './Company/company.component';
//@Component_Imports


export const SystemRoutes: Routes = [
    {
        path: '',
        children: [
            {
                path: 'user',
                component: UserComponent,
                data: { title: 'USER.USERMENU', breadcrumb: 'USER.USERMENU' }
            }
            , {
                path: 'role',
                component: RoleComponent,
                data: { title: 'ROLE.ROLEMENU', breadcrumb: 'ROLE.ROLEMENU' }
            },
             {
                path: 'auth',
                component: AuthComponent,
                 data: { title: 'AUTH.AUTHMENU', breadcrumb: 'AUTH.AUTHMENU' }
            }
            , {
                path: 'userrole',
                component: UserRoleComponent,
                data: { title: 'USERROLE.USERROLEMENU', breadcrumb: 'USERROLE.USERROLEMENU' }
            }
            , {
                path: 'roleauth',
                component: RoleAuthComponent,
                data: { title: 'ROLEAUTH.ROLEAUTHMENU', breadcrumb: 'ROLEAUTH.ROLEAUTHMENU' }
            }
            , {
                path: 'menu',
                component: MenuComponent,
                data: { title: 'MENU.MENUMENU', breadcrumb: 'MENU.MENUMENU' }
            },
            {
                path: 'email',
                component: EmailComponent,
                data: { title: 'EMAIL.EMAILMENU', breadcrumb: 'EMAIL.EMAILMENU' }
            },
            {
                path: 'emailconfig',
                component: EmailConfigComponent,
                data: { title: 'EMAILCONFIG.EMAILCONFIGMENU', breadcrumb: 'EMAILCONFIG.EMAILCONFIGMENU' }
            },
            {
                path: 'log',
                component: LogComponent,
                data: { title: 'LOG.LOGMENU', breadcrumb: 'LOG.LOGMENU' }
            }
            ,
            {
                path: 'emailattachment',
                component: EmailAttachmentComponent,
                data: { title: 'EMAILATTACHMENT.EMAILATTACHMENTMENU', breadcrumb: 'EMAILATTACHMENT.EMAILATTACHMENTMENU' }
            },
            {
                path: 'parameter',
                component: ParameterComponent,
                data: { title: 'PARAMETER.PARAMETERMENU', breadcrumb: 'PARAMETER.PARAMETERMENU' }
            },
            {
                path: 'counter',
                component: CounterComponent,
                data: { title: 'COUNTER.COUNTERMENU', breadcrumb: 'COUNTER.COUNTERMENU' }
            },
            {
                path: 'file',
                component: FileComponent,
                data: { title: 'FILE.FILEMENU', breadcrumb: 'FILE.FILEMENU' }
            },
            {
                path: 'userpageconfiguration',
                component: UserPageConfigurationComponent,
                data: { title: 'USERPAGECONFIGURATION.USERPAGECONFIGURATIONMENU', breadcrumb: 'USERPAGECONFIGURATION.USERPAGECONFIGURATIONMENU' }
            },
            {
                path: 'company',
                component: CompanyComponent,
                data: { title: 'COMPANY.COMPANYMENU', breadcrumb: 'COMPANY.COMPANYMENU' }
            }
            //@Component_Routes
        ]
    }
];
