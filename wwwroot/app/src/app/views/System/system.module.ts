import { NgModule, APP_INITIALIZER } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';

import { UserComponent } from './User/user.component';

import { ComfirmDialogComponent } from "../../shared/components/comfirm/comfirm.component";
import { PageConfigDialogComponent } from "../../shared/components/pageconfig/pageconfig.component";
import { SystemRoutes } from './system.routing';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSelectModule } from '@angular/material/select';
import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule } from '@angular/material/dialog';
import { UserDialogComponent } from './User/user-dialog.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { FlexLayoutModule } from '@angular/flex-layout';
import { HttpClientModule } from '@angular/common/http';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';

import { SessionService } from "../../shared/session.service";
import { RoleDialogComponent } from './Role/role-dialog.component';
import { RoleComponent } from './Role/role.component';
import { AuthComponent } from './Auth/auth.component';
import { AuthDialogComponent } from './Auth/auth-dialog.component';
import { UserRoleComponent } from './UserRole/userrole.component';
import { UserRoleDialogComponent } from './UserRole/userrole-dialog.component';
import { RoleAuthComponent } from './RoleAuth/roleauth.component';
import { RoleAuthDialogComponent } from './RoleAuth/roleauth-dialog.component';
import { MenuComponent } from './Menu/menu.component';
import { MenuDialogComponent } from './Menu/menu-dialog.component';
import { MatTabsModule } from '@angular/material/tabs';
import { MatListModule } from '@angular/material/list';
import { MatCardModule } from '@angular/material/card';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { EmailComponent } from './Email/email.component';
import { EmailDialogComponent } from './Email/email-dialog.component';
import { EmailConfigComponent } from './EmailConfig/emailconfig.component';
import { EmailConfigDialogComponent } from './EmailConfig/emailconfig-dialog.component';
import { LogComponent } from './Log/log.component';
import { LogDialogComponent } from './Log/log-dialog.component';
import { UserPageConfigurationComponent } from './UserPageConfiguration/userpageconfiguration.component';
import { UserPageConfigurationDialogComponent } from './UserPageConfiguration/userpageconfiguration-dialog.component';

import { EmailAttachmentComponent } from './EmailAttachment/emailattachment.component';
import { EmailAttachmentDialogComponent } from './EmailAttachment/emailattachment-dialog.component';
import { ParameterComponent } from './Parameter/parameter.component';
import { ParameterDialogComponent } from './Parameter/parameter-dialog.component';
import { CounterComponent } from './Counter/counter.component';
import { CounterDialogComponent } from './Counter/counter-dialog.component';
import { FileComponent } from './File/file.component';
import { FileDialogComponent } from './File/file-dialog.component';
import { SharedComponentsModule } from '../../shared/components/shared-components.module';
import { CompanyComponent } from './Company/company.component';
import { CompanyDialogComponent } from './Company/company-dialog.component';


//@Component_Imports

@NgModule({
    imports: [
        MatSidenavModule,
        MatGridListModule,
        MatButtonModule,
        MatDatepickerModule,
        MatNativeDateModule,
        MatSlideToggleModule,
        MatSelectModule,
        MatIconModule,
        FormsModule,
        ReactiveFormsModule,
        MatDialogModule,
        FlexLayoutModule,
        HttpClientModule,
        CommonModule,
        MatInputModule,
        MatPaginatorModule,
        MatSortModule,
        MatTableModule,
        NgxDatatableModule,
        MatProgressSpinnerModule,
        TranslateModule,
        MatTabsModule,
        MatListModule,
        MatCardModule,
        MatAutocompleteModule,
        SharedComponentsModule,
        RouterModule.forChild(SystemRoutes)
    ],

    declarations: [
        UserComponent,
        UserDialogComponent,
        RoleComponent,
        RoleDialogComponent,
        ComfirmDialogComponent,
        PageConfigDialogComponent,
        AuthComponent,
        AuthDialogComponent,
        RoleAuthComponent,
        RoleAuthDialogComponent,
        UserRoleComponent,
        UserRoleDialogComponent,
        MenuComponent,
        MenuDialogComponent,
        EmailComponent,
        EmailDialogComponent,
        EmailConfigComponent,
        EmailConfigDialogComponent,
        LogComponent,
        LogDialogComponent,
        EmailAttachmentComponent,
        EmailAttachmentDialogComponent,
        ParameterComponent,
        ParameterDialogComponent,
        CounterDialogComponent,
        CounterComponent,
        FileComponent,
        FileDialogComponent,
        UserPageConfigurationComponent,
        UserPageConfigurationDialogComponent,
        CompanyComponent,
        CompanyDialogComponent
        //FastAutoCompleteComponent
        //@Component_Declarations
    ],
    providers: [
        SessionService
    ]
})
export class SystemModule { }
