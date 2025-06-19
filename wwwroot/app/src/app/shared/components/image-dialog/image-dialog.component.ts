import { Component, Inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";



@Component({
    selector: 'image-dialog',
    templateUrl: 'image-dialog.component.html',
})
export class ImageDialogComponent {

    caption: string;
    imageList: [];
    currentImage: string;
    currentImageIndex: number;


    constructor(public dialogRef: MatDialogRef<ImageDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any) {

        this.imageList = data.imageList;
        this.currentImageIndex = data.currentImageIndex;

        this.currentImage = this.imageList[this.currentImageIndex];
        this.caption = data.caption;


        this.dialogRef.keydownEvents().subscribe(event => {
            if (event.keyCode === 37) {
                this.previousImage()
            }

            if (event.keyCode === 39) {
                this.nextImage();
            }
        });
    }

    nextImage() {
        if (this.imageList.length - 1 > this.currentImageIndex) {
            this.currentImageIndex++;
            this.currentImage = this.imageList[this.currentImageIndex];
        }
    }

    previousImage() {
        if (this.currentImageIndex != 0) {
            this.currentImageIndex--;
            this.currentImage = this.imageList[this.currentImageIndex];
        }
    }


}