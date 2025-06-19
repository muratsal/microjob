import { MatDialogRef } from "@angular/material/dialog";
import { CompanyData } from "./company.component";
import { Component, Inject } from "@angular/core";
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormControl } from '@angular/forms';
import { CompanyService } from './company.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { NavigationService } from '../../../shared/services/navigation.service'

@Component({
    selector: "company-dialog",
    templateUrl: "./company-dialog.component.html"
})
export class CompanyDialogComponent {

    row: CompanyData;
    isNew: boolean;
    constructor(public dialogRef: MatDialogRef<CompanyDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private companyService: CompanyService,
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

        this.companyService.save(this.row).subscribe((response: any) => {

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