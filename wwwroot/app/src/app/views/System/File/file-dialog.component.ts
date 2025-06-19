import { MatDialogRef } from "@angular/material/dialog";
import { FileData, FileTables } from "./file.component";
import { Component, Inject } from "@angular/core";
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormControl } from '@angular/forms';
import { FileService } from './file.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { NavigationService } from '../../../shared/services/navigation.service'

@Component({
    selector: "file-dialog",
    templateUrl: "./file-dialog.component.html"
})
export class FileDialogComponent {

    row: FileData;
    isNew: boolean;
    showTableInfo: boolean;
    tableNos: number[];
    fileTableEnums: any;
    isUploading: boolean = false;
    path: string;
    constructor(public dialogRef: MatDialogRef<FileDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private fileService: FileService,
        private translate: TranslateService,
        private navigationService: NavigationService,
        private snackbar: MatSnackBar) {
        this.row = data.row;
        this.row.displayByCustomer = false;
        this.isNew = data.isNew;
        this.showTableInfo = data.showTableInfo;
        this.path = data.path;
        this.fileTableEnums = FileTables;
        this.tableNos = Object.keys(this.fileTableEnums).filter(Number).map(x=> parseInt(x));
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

        this.fileService.save(this.row).subscribe((response: any) => {

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


    onFileSelected(event) {

        this.isUploading = true;

        const file: File = event.target.files[0];
        if (file) {
           
            this.fileService.uploadFile(event.target.files, this.row,this.path).subscribe((response: any) => {

                this.isUploading = false;

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

}