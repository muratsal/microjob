import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA, MatDialogConfig } from '@angular/material/dialog';
import { QueryInfo, Pager } from '../../../shared/query-info';
import { TranslateService } from '@ngx-translate/core';
import { environment } from "environments/environment";
import { ComfirmDialogComponent } from "../../../shared/components/comfirm/comfirm.component";
import { MatSnackBar } from '@angular/material/snack-bar';
import { NavigationService } from '../../../shared/services/navigation.service'
import { ActivatedRoute } from '@angular/router';
import { ParameterService } from '../../System/Parameter/parameter.service';
import { ParameterFilter } from '../../System/Parameter/parameter.component';
import { BulkQualityComfirmationData, BulkQualityItemComfirmationData, QualityControlData, QualityControlFilter } from '../QualityControl/qualitycontrol.component';
import { QualityControlItemData, QualityControlItemFilter } from '../QualityControlItem/qualitycontrolitem.component';
import { QualityControlItemService } from '../QualityControlItem/qualitycontrolitem.service';
import { QualityControlService } from '../QualityControl/qualitycontrol.service';
import { QualityControlItemDialogComponent } from '../QualityControlItem/qualitycontrolitem-dialog.component';
import { UtilitiesService } from '../../../utilities.service';
import { BulkQualityControlItemDialogComponent } from './bulkqualitycontrolitem-dialog.component';



@Component({
    selector: 'qualitycontroldetail-table',
    templateUrl: './qualitycontroldetail.component.html',
    styleUrls: ['./qualitycontroldetail.component.scss']
})
export class QualityControlDetailComponent implements OnInit {
    headerQueryInfo: QueryInfo;
    headerFilter: QualityControlFilter;
    headerData: QualityControlData;

    queryInfo: QueryInfo;
    showSpinner: boolean = false;
    displayedColumns: string[] = [];
    dataSource: any;
    qualityControlItemList: any;

    filter: QualityControlItemFilter;
    pageEvent: PageEvent;

    qualityControlId: number;
    environment: any;

    isMobileView: boolean;

    showBulkInputs: boolean;
    selectAll: boolean;

    selectedBulkQualityData: BulkQualityComfirmationData;
    qualityItemList: any;

    constructor(
        private qualityControlItemService: QualityControlItemService,
        private qualityControlService: QualityControlService,
        public dialog: MatDialog,
        public translate: TranslateService,
        private snackbar: MatSnackBar,
        private navigationService: NavigationService,
        private parameterService: ParameterService,
        private utilitiesService: UtilitiesService,
        private route: ActivatedRoute,
    ) {
        this.environment = environment;
    }

    ngOnInit() {

        this.isMobileView = this.utilitiesService.isMobileViewSelected();
      

        let ccid = this.route.snapshot.paramMap.get('ccid');
        this.qualityControlId = parseInt(ccid);

        this.headerFilter = new QualityControlFilter();
        this.getData();

        this.filter = new QualityControlItemFilter();
        this.filter.qualityControlId = this.qualityControlId;


        this.queryInfo = new QueryInfo();
        this.queryInfo.pager = new Pager();
        this.queryInfo.pager.currentPage = 0;
        this.queryInfo.pager.pageSize = 10;
        this.queryInfo.pager.totalCount = 0;

        if (!this.isMobileView) {
            this.displayedColumns = this.getDataConf().filter((x) => x.showColumn == true).map((c) => c.prop);
        }
        else {
            this.displayedColumns = ["mobileColumn"];
        }
        this.getDataItems(false);
    }


    ngAfterViewInit() {

    }
    //----------

    showBulkComfirmationInput() {
        this.showBulkInputs = !this.showBulkInputs;
        this.displayedColumns = this.getDataConf().filter((x) => x.showColumn == true).map((c) => c.prop);
    }

    selectAllItem() {

        this.qualityItemList = this.dataSource.filteredData;
        this.qualityItemList.forEach(x => {
          
            x.selected = this.selectAll;

        });
        this.dataSource = new MatTableDataSource(this.qualityItemList);
    }

    comfirmAllSelected() {
        this.openBulkComfirmationDialog();
    }


    openBulkComfirmationDialog() {

        this.qualityItemList = this.dataSource.filteredData;
        this.selectedBulkQualityData = new BulkQualityComfirmationData();
        this.selectedBulkQualityData.bulkQualityControlItemList = [];

        this.qualityItemList.forEach(x => {
            if (x.selected ) {
                let qualityItem = new BulkQualityItemComfirmationData();
                qualityItem.qualityControlItemId = x.qualityControlItemId;

                this.selectedBulkQualityData.bulkQualityControlItemList.push(qualityItem);
            }
        });


        this.dialog.open(BulkQualityControlItemDialogComponent, {
            width: '100%',
            hasBackdrop: true,
            disableClose: true,
            data: {
                row: this.selectedBulkQualityData,
            }
        }).afterClosed()
            .subscribe(response => {

                if (response.operation == "cancel") {
                    this.getDataItems(false);

                }

                else if (response.operation == "save") {
                    this.getDataItems(false);

                }

                this.selectAll = false;
                this.selectedBulkQualityData = null;
                this.showBulkInputs = false;
                this.displayedColumns = this.getDataConf().filter((x) => x.showColumn == true).map((c) => c.prop);

            });
    }

    //___________________


    save() {

        this.qualityControlService.save(this.headerData).subscribe((response: any) => {

            this.navigationService.sessionControl(response);

            if (response.isSuccess) {

                this.snackbar.open(this.translate.instant("GENERAL.SUCCESS"),
                    this.translate.instant(response.message),
                    {
                        horizontalPosition: 'start',
                        verticalPosition: 'bottom',
                        duration: 1000
                    });


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

    cancel() {

        let oid = this.route.snapshot.paramMap.get('ccid');
        this.qualityControlId = parseInt(oid);
        this.headerFilter = new QualityControlFilter();
        this.getData();
    }

    getData() {

        this.showSpinner = true;
        this.headerFilter.qualityControlId = this.qualityControlId;

        this.qualityControlService.getData(this.headerFilter, null, null, false)
            .subscribe((response: any) => {
                this.navigationService.sessionControl(response);
                this.showSpinner = false;
                if (response.isSuccess) {
                    if (response.data && response.data.length > 0)
                        this.headerData = response.data[0];
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

                    this.showSpinner = false;

                    this.snackbar.open(this.translate.instant("GENERAL.ERROR"),
                        this.translate.instant(error), {
                        horizontalPosition: 'start',
                        verticalPosition: 'bottom',
                        duration: 2000
                    });

                });
    }


    clearFilter() {
        this.filter = new QualityControlItemFilter();
        this.filter.qualityControlId = this.qualityControlId;
    }

    filterData() {
        this.queryInfo.pager.currentPage = 0;
        this.getDataItems(false);
    }

    downloadData() {
        this.getDataItems(true);
    }

    sortData($event) {
        this.queryInfo.orderby = $event.active;
        if ($event.direction == 'desc') this.queryInfo.orderby = '-' + this.queryInfo.orderby;
        this.queryInfo.pager.currentPage = 0;
        this.getDataItems(false);
    }

    onPaginateChange(event: PageEvent) {
        this.queryInfo.pager.currentPage = event.pageIndex;
        this.queryInfo.pager.pageSize = event.pageSize;
        this.getDataItems(false);
    }

    customerOrderItemQuickFilter(filterValue: string) {
        filterValue = filterValue.trim(); 
        this.dataSource.filter = filterValue;
    }

    getDataItems(isExport: boolean) {

        this.filter.qualityControlId = this.qualityControlId;

        this.showSpinner = true;
        let columnInfo = null;
        if (isExport) {

            columnInfo = this.getDataConf().filter(x => x.isExport == true)
                .map((x) => ({
                    prop: x.prop,
                    localText: this.translate.instant('QUALITYCONTROLITEM.' + x.translate)
                }));

        }


        this.qualityControlItemService.getData(this.filter, this.queryInfo, columnInfo, isExport)
            .subscribe((response: any) => {
                this.navigationService.sessionControl(response);
                this.showSpinner = false;
                if (response.isSuccess) {
                    if (isExport) {
                        window.open(environment.apiURL + "/Download/FromCacheByKey?key=" + response.key, '_blank');
                    }
                    else {

                        this.queryInfo.pager.totalCount = response.totalCount;
                        this.dataSource = new MatTableDataSource(response.data);
                        this.dataSource.filterPredicate =
                            (data: QualityControlItemData, filter: string) => {
                                return (data.supplierName.indexOf(filter) != -1) ||
                                    (data.productName.indexOf(filter) != -1) ||
                                    (data.customerOrderReference.indexOf(filter) != -1) ||
                                    (data.productCode.indexOf(filter) != -1);
                            };
                    }
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

                    this.showSpinner = false;

                    this.snackbar.open(this.translate.instant("GENERAL.ERROR"),
                        this.translate.instant(error), {
                        horizontalPosition: 'start',
                        verticalPosition: 'bottom',
                        duration: 2000
                    });

                });
    }

    newItemDialog() {

        let emptyData = new QualityControlItemData();

        emptyData.qualityControlId = 0;
        emptyData.qualityControlId = this.headerData.qualityControlId;
        //emptyData.ccoiId = 0;
        emptyData.status = 1;
        emptyData.description = '';
        emptyData.createdDate = new Date();
        emptyData.createdUser = 0;
        emptyData.updatedDate = new Date();
        emptyData.updatedUser = 0;

        this.openDialog(emptyData, true, true);
    }

    editItemDialog(row: QualityControlItemData) {
        this.openDialog(row, false, true);
    }


    

    openDialog(row: QualityControlItemData, isNew: boolean, hasParent: boolean) {
       
        this.dialog.open(QualityControlItemDialogComponent, {
            width: '100%',
            hasBackdrop: true,
            disableClose: true,
            data: {
                row: row,
                isNew: isNew,
                hasParent: hasParent
            },
            panelClass: 'fullscreen-dialog',
            height: '100vh'
        }).afterClosed()
            .subscribe(response => {

                if (response.operation == "cancel") {
                    this.getDataItems(false);
                }

                else if (response.operation == "delete") {
                    this.delete(response.data);
                }

                else if (response.operation == "save") {
                    this.getDataItems(false);
                }

            });
    }


    delete(data: QualityControlItemData) {

        let deleteRow = data;
        this.dialog.open(ComfirmDialogComponent, {
            hasBackdrop: true
        }).afterClosed()
            .subscribe(response => {
                if (response.comfirm == true) {
                    this.qualityControlItemService.delete(deleteRow).subscribe((response: any) => {
                        this.navigationService.sessionControl(response);
                        if (response.isSuccess) {
                            this.snackbar.open(this.translate.instant("GENERAL.SUCCESS"),
                                this.translate.instant(response.message), {
                                horizontalPosition: 'start',
                                verticalPosition: 'bottom',
                                duration: 2000
                            });
                            this.getDataItems(false);
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

                            this.showSpinner = false;

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




    getDataConf() {
        return [

            {
                prop: 'Actions',
                translate: 'ACTIONS',
                showColumn: true,
                isExport: false,
                localText: ''
            },
            {
                prop: 'qualityControlItemId',
                translate: 'QUALITYCONTROLITEMID',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'qualityControlId',
                translate: 'QUALITYCONTROLID',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'ccoiId',
                translate: 'CCOIID',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'orderItemNo',
                translate: 'ORDERITEMNO',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'productImage',
                translate: 'PRODUCTIMAGE',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'productName',
                translate: 'PRODUCTNAME',
                showColumn: true,
                isExport: true,
                localText: ''
            }
            ,
            {
                prop: 'productDescription',
                translate: 'PRODUCTDESCRIPTION',
                showColumn: true,
                isExport: true,
                localText: ''
            }
            ,
            {
                prop: 'quantity',
                translate: 'QUANTITY',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'supplierName',
                translate: 'SUPPLIERNAME',
                showColumn: true,
                isExport: true,
                localText: ''
            }
            ,
            {
                prop: 'status',
                translate: 'STATUS',
                showColumn: true,
                isExport: true,
                localText: ''
            }
            ,
            {
                prop: 'description',
                translate: 'DESCRIPTION',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'numberOfQualityImage',
                translate: 'NUMBEROFQUALITYIMAGE',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'customerOrderReference',
                translate: 'CUSTOMERORDERREFERENCE',
                showColumn: true,
                isExport: true,
                localText: ''
            }
            ,
            {
                prop: 'brandName',
                translate: 'BRANDNAME',
                showColumn: true,
                isExport: true,
                localText: ''
            }
            ,
            {
                prop: 'createdDate',
                translate: 'CREATEDDATE',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'createdUser',
                translate: 'CREATEDUSER',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'createdUserText',
                translate: 'CREATEDUSERTEXT',
                showColumn: false,
                isExport: true,
                localText: ''
            }
            ,
            {
                prop: 'updatedDate',
                translate: 'UPDATEDDATE',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'updatedUser',
                translate: 'UPDATEDUSER',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'updatedUserText',
                translate: 'UPDATEDUSERTEXT',
                showColumn: true,
                isExport: true,
                localText: ''
            }


        ];
    }

    onChange($event, row, field) {
        row[field] = $event;
    }

    excelProformaDownload() {
        window.location.href = environment.apiURL + "/QualityControl/GetQualityControlProformaExcel?qualityControlId=" + this.headerData.qualityControlId;
    }

    pdfProformaDownload() {
        window.location.href = environment.apiURL + "/QualityControl/GetQualityControlProformaExcel?qualityControlId=" + this.headerData.qualityControlId;
    }

    changeView() {
        this.isMobileView = !this.isMobileView;

        if (!this.isMobileView) {
            this.displayedColumns = this.getDataConf().filter((x) => x.showColumn == true).map((c) => c.prop);
        }
        else {
            this.displayedColumns = ["mobileColumn"];
        }

        this.utilitiesService.setMobileView(this.isMobileView);
    }

}



