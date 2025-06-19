import { Component, Inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";



@Component({
    selector: 'comfirm-dialog',
    templateUrl: 'comfirm.component.html',
})
export class ComfirmDialogComponent {

    message: string;
    action: string;

    constructor(public dialogRef: MatDialogRef<ComfirmDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any) {

        this.message = 'GENERAL.AREYOUSUREDELETE';
        this.action = 'GENERAL.DELETE';
        if (data) {

            if (data.message) this.message = data.message;
            if (data.action) this.action = data.action;
        }

    }

    comfirmDelete(comfirm: boolean) {
        this.dialogRef.close({ comfirm: comfirm });
    }

}