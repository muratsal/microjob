import { MatDialogRef } from "@angular/material/dialog";
import { Component, Inject } from "@angular/core";
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormControl } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { NavigationService } from '../../../shared/services/navigation.service'
import { environment } from "environments/environment";
import { MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { MY_DATE_FORMATS } from '../../../shared/dateformat'
import { BulkQualityComfirmationData } from "../QualityControl/qualitycontrol.component";
import { QualityControlItemService } from "../QualityControlItem/qualitycontrolitem.service";

@Component({
    selector: "bulkqualitycontrolitem-dialog",
    templateUrl: "./bulkqualitycontrolitem-dialog.component.html",
    providers: [
        { provide: MAT_DATE_FORMATS, useValue: MY_DATE_FORMATS },
    ]
})
export class BulkQualityControlItemDialogComponent {

    row: BulkQualityComfirmationData;
    environment: any;
    selectedBulkOperation: number;
    controlStatus: number;
    constructor(public dialogRef: MatDialogRef<BulkQualityControlItemDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private translate: TranslateService,
        private navigationService: NavigationService,
        private qualityControlItemService: QualityControlItemService,
        private snackbar: MatSnackBar) {
        this.row = data.row;
        this.environment = environment;
    }

    ngOnInit() {

      
    }

    rebuildForm() {

    }

    close() {

        this.dialogRef.close({ data: this.row, operation: "cancel" });
    }



    save() {

        let requiredMessage = this.translate.instant("GENERAL.REQUIREDFIELDSMUSTFILLED");
        let isValid: boolean = true;
        let fieldName: string;

        if (!this.selectedBulkOperation) {
                fieldName = "No Operation Selected"
                isValid = false;
        }

        else if (!this.row.bulkQualityControlItemList || this.row.bulkQualityControlItemList.length == 0) {
            fieldName = this.translate.instant("No Item Selected");
            isValid = false;
        }

        else if (this.selectedBulkOperation == 2 && !this.controlStatus ) {
            fieldName = this.translate.instant("Status");
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

        switch (this.selectedBulkOperation) {
            case 1: {
                this.deleteBulkQualityControlComfirmation();
                break;
            }
            case 2: {
                this.bulkStatusQualityControlItem();
                break;
            }
           
        }
    }
    
    deleteBulkQualityControlComfirmation() {
        let qualityItems = this.row.bulkQualityControlItemList.map(x => x.qualityControlItemId);
        this.qualityControlItemService.bulkDelete(qualityItems).subscribe((response: any) => {

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


    bulkStatusQualityControlItem() {
        let qualityItems = this.row.bulkQualityControlItemList.map(x => x.qualityControlItemId);
       
        this.qualityControlItemService.bulkStatusQualityControlItem(qualityItems, this.controlStatus).subscribe((response: any) => {

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