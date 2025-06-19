import { MatDialogRef } from "@angular/material/dialog";
import { QualityControlItemData } from "./qualitycontrolitem.component";
import { Component, Inject } from "@angular/core";
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormControl } from '@angular/forms';
import { QualityControlItemService } from './qualitycontrolitem.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { NavigationService } from '../../../shared/services/navigation.service'
import { environment } from "environments/environment";
import { ComfirmedCustomerOrderItemData, ComfirmedCustomerOrderItemFilter } from "../../Customer/ComfirmedCustomerOrderItem/comfirmedcustomerorderitem.component";
import { ComfirmedCustomerOrderItemService } from "../../Customer/ComfirmedCustomerOrderItem/comfirmedcustomerorderitem.service";
import { QualityControlItemImageData, QualityControlItemImageFilter } from "../QualityControlItemImage/qualitycontrolitemimage.component";
import { Pager, QueryInfo } from "../../../shared/query-info";
import { QualityControlItemImageService } from "../QualityControlItemImage/qualitycontrolitemimage.service";
import { ComfirmDialogComponent } from "../../../shared/components/comfirm/comfirm.component";
import { ImageDialogComponent } from "../../../shared/components/image-dialog/image-dialog.component";
import { UtilitiesService } from "../../../utilities.service";
import { JwtAuthService } from "../../../shared/services/auth/jwt-auth.service";

@Component({
    selector: "qualitycontrolitem-dialog",
    templateUrl: "./qualitycontrolitem-dialog.component.html"
})
export class QualityControlItemDialogComponent {

    row: QualityControlItemData;
    isNew: boolean;
    environment: any;
    qualityControlItemImageList: QualityControlItemImageData[];
    filteredQualityControlItemImageList: QualityControlItemImageData[];
    description: string;
    qualityControlItemImageQuickFilter: string;
    qualityImageShowSelection: boolean;
    selectAllImages: boolean;
    constructor(public dialogRef: MatDialogRef<QualityControlItemDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private dialog: MatDialog,
        private qualityControlItemService: QualityControlItemService,
        private translate: TranslateService,
        private navigationService: NavigationService,
        private qualityControlItemImageService: QualityControlItemImageService,
        private comfirmedCustomerOrderItemService: ComfirmedCustomerOrderItemService,
        private utilitiesService: UtilitiesService,
        private authService: JwtAuthService,
        private snackbar: MatSnackBar) {
        this.row = data.row;
        this.isNew = data.isNew;
        this.environment = environment;
        this.description = "";
        this.qualityControlItemImageQuickFilter = "";
        this.qualityImageShowSelection = false;
        this.selectAllImages = false;
    }

    ngOnInit() {

        if (!this.isNew) {
            this.getQualityControlItemImageData();
        }

        setTimeout(function () {
            let elem: any = document.getElementsByClassName("fullscreen-dialog")[0];
            elem.style.maxWidth = "100vw";
        }, 10);

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

        this.qualityControlItemService.save(this.row).subscribe((response: any) => {

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
        if (field == 'ccoiId') {
            this.onComfirmedOrderItemChange($event);
        }
    }

    selectedCcoi: ComfirmedCustomerOrderItemData;

    onComfirmedOrderItemChange(ccoiId: number) {
        if (!ccoiId) return;
        let ccoiFilter = new ComfirmedCustomerOrderItemFilter();
        ccoiFilter.ccoiId = ccoiId;
        this.comfirmedCustomerOrderItemService.getData(ccoiFilter, null, null, false).subscribe((response: any) => {

            this.navigationService.sessionControl(response);

            if (response.isSuccess) {

                this.selectedCcoi = response.data[0];

                this.row.productDescription = this.selectedCcoi.description;
                this.row.capacityInCbm = this.selectedCcoi.capacityInCbm;
                this.row.customerOrderReference = this.selectedCcoi.customerOrderReference;
                this.row.quantity = this.selectedCcoi.quantity;
                this.row.unitText = this.selectedCcoi.unitText;
                this.row.deadline = this.selectedCcoi.deadline;

                this.row.productId = this.selectedCcoi.productId;
                this.row.productCode = this.selectedCcoi.productCode;
                this.row.productName = this.selectedCcoi.productName;
                this.row.supplierId = this.selectedCcoi.supplierId;
                this.row.supplierName = this.selectedCcoi.supplierName;
                this.row.productImage = this.selectedCcoi.productImage;

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

    statusChange(event) {
        if (event.value == 2) {
            this.row.controlledQuantity = this.row.requiredQuantity;
        }
    }


    onLoadingImageFileSelected(event) {

        let proxy = this;
        for (let i: number = 0; i < event.target.files.length; i++) {

            let file: File = event.target.files[i];
            if (file.type.match(/image.*/)) {
                console.log('An image has been loaded');

                let reader = new FileReader();
                reader.onload = function (readerEvent) {
                    let image = new Image();
                    image.onload = (imageEvent) => {

                        let resizedImage = proxy.utilitiesService.resizeImage(image, 1024);

                        proxy.qualityControlItemImageService.uploadImage(resizedImage, proxy.row.qualityControlItemId, proxy.description).subscribe((response: any) => {
                            if (response.isSuccess) {
                                proxy.qualityControlItemImageList.unshift(response.data);
                                proxy.onFilterQualityControlItemImage()
                            }
                        });

                    }
                    image.src = readerEvent.target.result as any;
                }
                reader.readAsDataURL(file);
            }
        }
    }

    getQualityControlItemImageData() {

        let qualityControlItemImageFilter: QualityControlItemImageFilter = new QualityControlItemImageFilter();
        qualityControlItemImageFilter.qualityControlItemId = this.row.qualityControlItemId;

        let qualityControlItemImageQueryInfo: QueryInfo = new QueryInfo();
        qualityControlItemImageQueryInfo.pager = new Pager();
        qualityControlItemImageQueryInfo.pager.currentPage = 0;
        qualityControlItemImageQueryInfo.pager.pageSize = 150;
        qualityControlItemImageQueryInfo.orderby = "-QciImageId";


        this.qualityControlItemImageService.getData(qualityControlItemImageFilter, qualityControlItemImageQueryInfo, null, false)
            .subscribe((response: any) => {
                this.navigationService.sessionControl(response);
                if (response.isSuccess) {
                    this.qualityControlItemImageList = response.data;
                    this.filteredQualityControlItemImageList = this.qualityControlItemImageList;
                }
                else {

                    this.snackbar.open(this.translate.instant("GENERAL.ERROR"),
                        this.translate.instant(response.error), {
                        horizontalPosition: 'start',
                        verticalPosition: 'bottom',
                        duration: 2000
                    });
                }
            },
                (error) => {

                    this.snackbar.open(this.translate.instant("GENERAL.ERROR"),
                        this.translate.instant(error), {
                        horizontalPosition: 'start',
                        verticalPosition: 'bottom',
                        duration: 2000
                    });

                });
    }

    deleteQualityControlItemImage(data: QualityControlItemImageData) {

        let deleteRow = data;
        this.dialog.open(ComfirmDialogComponent, {
            hasBackdrop: true
        }).afterClosed()
            .subscribe(response => {
                if (response.comfirm == true) {
                    this.qualityControlItemImageService.delete(deleteRow).subscribe((response: any) => {
                        this.navigationService.sessionControl(response);
                        if (response.isSuccess) {
                            this.snackbar.open(this.translate.instant("GENERAL.SUCCESS"),
                                this.translate.instant(response.message), {
                                horizontalPosition: 'start',
                                verticalPosition: 'bottom',
                                duration: 2000
                            });
                            this.getQualityControlItemImageData();
                        }
                        else if (!response.isSuccess) {
                            this.snackbar.open(this.translate.instant("GENERAL.ERROR"),
                                response.errorMessage, {
                                horizontalPosition: 'start',
                                verticalPosition: 'bottom',
                                duration: 2000
                            });
                        }
                    },
                        (error) => {

                            this.snackbar.open(this.translate.instant("GENERAL.ERROR"),
                                error, {
                                horizontalPosition: 'start',
                                verticalPosition: 'bottom',
                                duration: 2000
                            });

                        });
                }
                else {
                    this.snackbar.open(this.translate.instant("GENERAL.CANCELED"), "", {
                        horizontalPosition: 'start',
                        verticalPosition: 'bottom',
                        duration: 2000
                    });
                }
            });
    }

    qualityControlItemImageViewDialog(index: number) {

        this.dialog.open(ImageDialogComponent, {
            hasBackdrop: true,
            data: {
                caption: "QUALITYCONTROLITEMIMAGE.QUALITYCONTROLITEMIMAGE",
                imageList: this.filteredQualityControlItemImageList.map(x => environment.imagePath + x.imagePath),
                currentImageIndex: index
            }
        }).afterClosed()
            .subscribe(response => {

            });
    }

    filterQualityControlItemImage(filter: string) {
       return  this.qualityControlItemImageList.filter(x => x.description.indexOf(filter) > -1)
    }

    onFilterQualityControlItemImage() {

        this.filteredQualityControlItemImageList = this.filterQualityControlItemImage(this.qualityControlItemImageQuickFilter);
    }


    selectAllImagesClick() {

        //this.selectAllImages = !this.selectAllImages;
        this.qualityControlItemImageList.forEach(x => x.selected = this.selectAllImages);

    }

    qualityImageShowSelectionClick() {
        this.qualityImageShowSelection = !this.qualityImageShowSelection;
        this.qualityControlItemImageList.forEach(x => x.selected = false);
    }

    bulkDeleteQualityControlItemImage() {

        let selectedIds = this.qualityControlItemImageList.filter(x => x.selected == true).map(x => x.qciImageId);

        this.dialog.open(ComfirmDialogComponent, {
            hasBackdrop: true
        }).afterClosed()
            .subscribe(response => {
                if (response.comfirm == true) {
                    this.qualityControlItemImageService.bulkDelete(selectedIds).subscribe((response: any) => {
                        this.navigationService.sessionControl(response);
                        if (response.isSuccess) {
                            this.snackbar.open(this.translate.instant("GENERAL.SUCCESS"),
                                this.translate.instant(response.message), {
                                horizontalPosition: 'start',
                                verticalPosition: 'bottom',
                                duration: 2000
                            });
                            this.getQualityControlItemImageData();
                            this.qualityImageShowSelection = false;
                        }
                        else if (!response.isSuccess) {
                            this.snackbar.open(this.translate.instant("GENERAL.ERROR"),
                                response.errorMessage, {
                                horizontalPosition: 'start',
                                verticalPosition: 'bottom',
                                duration: 2000
                            });
                        }
                    },
                        (error) => {

                            this.snackbar.open(this.translate.instant("GENERAL.ERROR"),
                                error, {
                                horizontalPosition: 'start',
                                verticalPosition: 'bottom',
                                duration: 2000
                            });

                        });
                }
                else {
                    this.snackbar.open(this.translate.instant("GENERAL.CANCELED"), "", {
                        horizontalPosition: 'start',
                        verticalPosition: 'bottom',
                        duration: 2000
                    });
                }
            });
    }

    downloadQualityImages() {
        window.location.href = environment.apiURL + "/QualityControlItemImage/DowmloadQualityImageZipByQControlId?qualityControlItemId=" + this.row.qualityControlItemId + "&session=" + this.authService.getToken();
    }
    

}