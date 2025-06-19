import { Component, Inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { UserPageConfigurationData } from '../../../views/System/UserPageConfiguration/userpageconfiguration.component';
import { UserPageConfigurationService } from '../../../views/System/UserPageConfiguration/userpageconfiguration.service';



@Component({
    selector: 'pageconfig-dialog',
    templateUrl: 'pageconfig.component.html',
})
export class PageConfigDialogComponent {

    columnList: string[];
    displayedColumns: string[];
    pageSize: number;
    orderBy: string;
    pageName: string;

    constructor(public dialogRef: MatDialogRef<PageConfigDialogComponent>,
        public userPageConfigurationService: UserPageConfigurationService,
        private snackbar: MatSnackBar,
        public translate: TranslateService,
        @Inject(MAT_DIALOG_DATA) public data: any) {

        this.columnList = data.columnList;
        this.displayedColumns = data.displayedColumns;
        this.pageSize = data.pageSize;
        this.orderBy = data.orderBy;
        this.pageName = data.pageName;
    }



    save() {

        let data: UserPageConfigurationData = new UserPageConfigurationData();

        data.confData = JSON.stringify({
            displayedColumns: this.displayedColumns,
            pageSize: this.pageSize,
            orderBy: this.orderBy
        });

        data.confPage = this.pageName;
        data.userId = JSON.parse(localStorage.getItem("USER_INFO")).userId;

        this.userPageConfigurationService.saveUserConfig(data).subscribe((response: any) => {


            if (response.isSuccess) {

                this.snackbar.open(this.translate.instant("GENERAL.SUCCESS"),
                    this.translate.instant(response.message),
                    {
                        horizontalPosition: 'start',
                        verticalPosition: 'bottom',
                        duration: 1000
                    });

                this.dialogRef.close({ displayedColumns: this.displayedColumns });
            }
            else {

                this.snackbar.open(this.translate.instant("GENERAL.ERROR"),
                    this.translate.instant(response.errorMessage),
                    {
                        horizontalPosition: 'start',
                        verticalPosition: 'bottom',
                        duration: 1000
                    });
            }

        },
            (error) => {
                this.snackbar.open(this.translate.instant("GENERAL.ERROR"),
                    this.translate.instant(error), {
                    horizontalPosition: 'start',
                    verticalPosition: 'bottom',
                    duration: 2000
                });

            });
           
    }



}