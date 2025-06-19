import { MatDialogRef } from "@angular/material/dialog";
import { QualityControlData } from "./qualitycontrol.component";
import { Component, Inject } from "@angular/core";
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormControl } from '@angular/forms';
import { QualityControlService } from './qualitycontrol.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { NavigationService } from '../../../shared/services/navigation.service'

@Component({
    selector: "qualitycontrol-dialog",
    templateUrl: "./qualitycontrol-dialog.component.html"
})
export class QualityControlDialogComponent {

    row: QualityControlData;
    isNew: boolean;
    constructor(public dialogRef: MatDialogRef<QualityControlDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private qualityControlService: QualityControlService,
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

        let requiredMessage = this.translate.instant("GENERAL.REQUIREDFIELDSMUSTFILLED");
        let isValid: boolean = true;
        let fieldName: string;
        if (!this.row.assignee) {
            fieldName = this.translate.instant("QUALITYCONTROL.ASSIGNEE");
            isValid = false;
        }
     

        if (!isValid) {

            requiredMessage += "[ " + fieldName + " ]";
            this.snackbar.open(this.translate.instant("GENERAL.ERROR"),
                requiredMessage,
                {
                    horizontalPosition: 'start',
                    verticalPosition: 'bottom',
                    duration: 2500
                });

            return;
        }

        this.qualityControlService.save(this.row).subscribe((response: any) => {

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

    onChange($event, row, field) {
        row[field] = $event;
    }

}