import { MatDialogRef } from "@angular/material/dialog";
import { UserRoleData } from "./userrole.component";
import { Component, Inject } from "@angular/core";
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormControl } from '@angular/forms';
import { UserRoleService } from './userrole.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';


@Component({
    selector: "userrole-dialog",
    templateUrl: "./userrole-dialog.component.html"
})
export class UserRoleDialogComponent {

    row: UserRoleData;
    isNew: boolean;
    constructor(public dialogRef: MatDialogRef<UserRoleDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private userRoleService: UserRoleService,
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

        this.userRoleService.save(this.row).subscribe((response: any) => {

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