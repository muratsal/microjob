import { MatDialogRef } from "@angular/material/dialog";
import { UserPageConfigurationData } from "./userpageconfiguration.component";
import { Component, Inject } from "@angular/core";
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormControl } from '@angular/forms';
import { UserPageConfigurationService } from './userpageconfiguration.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { NavigationService } from '../../../shared/services/navigation.service'

@Component({
    selector: "userpageconfiguration-dialog",
    templateUrl: "./userpageconfiguration-dialog.component.html"
})
export class UserPageConfigurationDialogComponent {

    row: UserPageConfigurationData;
    isNew: boolean;
    constructor(public dialogRef: MatDialogRef<UserPageConfigurationDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private userPageConfigurationService: UserPageConfigurationService,
        private translate : TranslateService,
        private navigationService : NavigationService,
        private snackbar: MatSnackBar) {
        this.row = data.row;
        this.isNew = data.isNew;
    }

    ngOnInit() {


    }

    rebuildForm() {

    }

    close() {

        this.dialogRef.close({ data: this.row, operation: "cancel" });
    }

    delete() {

        this.dialogRef.close({ data: this.row, operation: "delete" });
    }

    save() {

        this.userPageConfigurationService.save(this.row).subscribe((response: any) => {

            this.navigationService.sessionControl(response);

            if (response.isSuccess) {

                this.snackbar.open(this.translate.instant("GENERAL.SUCCESS"),
                                    this.translate.instant(response.message), 
                 {
                    horizontalPosition: 'start',
                    verticalPosition: 'bottom',
                    duration: 1000
                });

                this.dialogRef.close({ data: this.row, operation: "save" });
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

        }, (error) => {

            this.snackbar.open(this.translate.instant("GENERAL.ERROR"),
                this.translate.instant(error), {
                horizontalPosition: 'start',
                verticalPosition: 'bottom',
                duration: 2000
            });

        });


    }

}