import { MatDialogRef } from "@angular/material/dialog";
import { RoleAuthData } from "./roleauth.component";
import { Component, Inject } from "@angular/core";
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormControl } from '@angular/forms';
import { RoleAuthService } from './roleauth.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: "roleauth-dialog",
    templateUrl: "./roleauth-dialog.component.html"
})
export class RoleAuthDialogComponent {

    row: RoleAuthData;
    isNew: boolean;
    constructor(public dialogRef: MatDialogRef<RoleAuthDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private roleAuthService: RoleAuthService,
        private translate : TranslateService,
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

        this.roleAuthService.save(this.row).subscribe((response: any) => {

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

        });


    }

}