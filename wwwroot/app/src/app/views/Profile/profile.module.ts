import { NgModule, APP_INITIALIZER } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';


import { ComfirmDialogComponent } from "../../shared/components/comfirm/comfirm.component";
import { ProfileRoutes } from './profile.routing';
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
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { FlexLayoutModule } from '@angular/flex-layout';
import { HttpClientModule } from '@angular/common/http';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';

import { SessionService } from "../../shared/session.service";
import { SharedComponentsModule } from '../../shared/components/shared-components.module'

import { MatTabsModule } from '@angular/material/tabs';
import { MatListModule } from '@angular/material/list';
import { MatCardModule } from '@angular/material/card';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { OverviewComponent } from './Overview/overview.component';





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
        RouterModule.forChild(ProfileRoutes)
    ],

    declarations: [
       OverviewComponent
        //FastAutoCompleteComponent
        //@Component_Declarations
    ],
    providers: [
        SessionService
    ]
})
export class ProfileModule { }
