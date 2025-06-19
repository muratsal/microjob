import { MatDialogRef } from "@angular/material/dialog";
import { InventoryTransactionData } from "./inventorytransaction.component";
import { Component, Inject } from "@angular/core";
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormControl } from '@angular/forms';
import { InventoryTransactionService } from './inventorytransaction.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { NavigationService } from '../../../shared/services/navigation.service'
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from "@angular/material/core";
import { MAT_MOMENT_DATE_ADAPTER_OPTIONS, MAT_MOMENT_DATE_FORMATS, MomentDateAdapter } from "@angular/material-moment-adapter";

@Component({
    selector: "inventorytransaction-dialog",
    templateUrl: "./inventorytransaction-dialog.component.html",
    providers: [
        { provide: MAT_DATE_LOCALE, useValue: 'tr-TR' },
        { provide: MAT_MOMENT_DATE_ADAPTER_OPTIONS, useValue: { useUtc: true } },
        { provide: MAT_DATE_FORMATS, useValue: MAT_MOMENT_DATE_FORMATS },
        { provide: DateAdapter, useClass: MomentDateAdapter, deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS] },
    ]
})
export class InventoryTransactionDialogComponent {

    row: InventoryTransactionData;
    isNew: boolean;
    constructor(public dialogRef: MatDialogRef<InventoryTransactionDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private inventoryTransactionService: InventoryTransactionService,
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

        this.inventoryTransactionService.save(this.row).subscribe((response: any) => {

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