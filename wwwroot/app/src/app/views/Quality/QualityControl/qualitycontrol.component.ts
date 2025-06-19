import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { QualityControlService } from './qualitycontrol.service';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA, MatDialogConfig } from '@angular/material/dialog';
import { QualityControlDialogComponent } from './qualitycontrol-dialog.component';
import { QueryInfo, Pager } from '../../../shared/query-info';
import { TranslateService } from '@ngx-translate/core';
import { environment } from "environments/environment";
import { ComfirmDialogComponent } from "../../../shared/components/comfirm/comfirm.component";
import { MatSnackBar } from '@angular/material/snack-bar';
import { NavigationService } from '../../../shared/services/navigation.service'
import { Router } from '@angular/router';
import { UtilitiesService } from '../../../utilities.service';



@Component({
    selector: 'qualitycontrol-table',
    templateUrl: './qualitycontrol.component.html',
    styleUrls: ['./qualitycontrol.component.scss']
})
export class QualityControlComponent implements OnInit {

    queryInfo: QueryInfo;
    showSpinner: boolean = false;
    displayedColumns: string[] = [];
    dataSource: any;
    filter: QualityControlFilter;
    pageEvent: PageEvent;
    isMobileView: boolean;


    constructor(private qualityControlService: QualityControlService,
        public dialog: MatDialog,
        public translate: TranslateService,
        private snackbar: MatSnackBar,
        private utilitiesService: UtilitiesService,
        private router: Router,
        private navigationService: NavigationService) { }

    ngOnInit() {
        this.isMobileView = this.utilitiesService.isMobileViewSelected();

        this.filter = new QualityControlFilter();
        this.queryInfo = new QueryInfo();
        this.queryInfo.pager = new Pager();
        this.queryInfo.pager.currentPage = 0;
        this.queryInfo.pager.pageSize = 10;
        this.queryInfo.pager.totalCount = 0;

        this.filter = this.utilitiesService.getFilterCache("QualityControlFilter") as QualityControlFilter;
        if (!this.filter) this.filter = new QualityControlFilter();

        if (!this.isMobileView) {
            this.displayedColumns = this.getDataConf().filter((x) => x.showColumn == true).map((c) => c.prop);
        }
        else {
            this.displayedColumns = ["mobileColumn"];
        }
        this.getData(false);
    }

    ngAfterViewInit() {

    }

    clearFilter() {
        this.filter = new QualityControlFilter();
    }

    filterData() {
        this.queryInfo.pager.currentPage = 0;
        this.getData(false);
    }

    downloadData() {
        this.getData(true);
    }

    sortData($event) {


        this.queryInfo.orderby = $event.active;
        if ($event.direction == 'desc') this.queryInfo.orderby = '-' + this.queryInfo.orderby;
        this.queryInfo.pager.currentPage = 0;
        this.getData(false);
    }

    onPaginateChange(event: PageEvent) {
        this.queryInfo.pager.currentPage = event.pageIndex;
        this.queryInfo.pager.pageSize = event.pageSize;
        this.getData(false);
    }

    getData(isExport: boolean) {
        this.showSpinner = true;
        let columnInfo = null;
        if (isExport) {

            columnInfo = this.getDataConf().filter(x => x.isExport == true)
                .map((x) => ({
                    prop: x.prop,
                    localText: this.translate.instant('QUALITYCONTROL.' + x.translate)
                }));

        }


        this.qualityControlService.getData(this.filter, this.queryInfo, columnInfo, isExport)
            .subscribe((response: any) => {
                this.navigationService.sessionControl(response);
                this.utilitiesService.setFilterCache("QualityControlFilter", this.filter);
                this.showSpinner = false;
                if (response.isSuccess) {
                    if (isExport) {
                        window.open(environment.apiURL + "/Download/FromCacheByKey?key=" + response.key, '_blank');
                    }
                    else {

                        this.queryInfo.pager.totalCount = response.totalCount;
                        this.dataSource = new MatTableDataSource(response.data);
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

        let emptyData = new QualityControlData();

        emptyData.qualityControlId = 0;
        //emptyData.customerId = 0;
        emptyData.status = 1;
        emptyData.description = '';
        //emptyData.assignee = 0;
        emptyData.createdDate = new Date();
        emptyData.createdUser = 0;
        emptyData.updatedDate = new Date();
        emptyData.updatedUser = 0;

        this.openDialog(emptyData, true);
    }

    editItemDialog(row: QualityControlData) {
        //this.openDialog(row, false);
        this.router.navigateByUrl('/quality/qualitycontroldetail/' + row.qualityControlId);
    }

    openDialog(row: QualityControlData, isNew: boolean) {

        this.dialog.open(QualityControlDialogComponent, {
            width: '100%',
            hasBackdrop: true,
            disableClose: true,
            data: {
                row: row,
                isNew: isNew
            }
        }).afterClosed()
            .subscribe(response => {

                if (response.operation == "cancel") {
                    this.getData(false);
                }

                else if (response.operation == "delete") {
                    this.delete(response.data);
                }

                else if (response.operation == "save") {
                    this.getData(false);
                }

            });
    }


    delete(data: QualityControlData) {

        let deleteRow = data;
        this.dialog.open(ComfirmDialogComponent, {
            hasBackdrop: true
        }).afterClosed()
            .subscribe(response => {
                if (response.comfirm == true) {
                    this.qualityControlService.delete(deleteRow).subscribe((response: any) => {
                        this.navigationService.sessionControl(response);
                        if (response.isSuccess) {
                            this.snackbar.open(this.translate.instant("GENERAL.SUCCESS"),
                                this.translate.instant(response.message), {
                                horizontalPosition: 'start',
                                verticalPosition: 'bottom',
                                duration: 2000
                            });
                            this.getData(false);
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
                prop: 'qualityControlId',
                translate: 'QUALITYCONTROLID',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'customerId',
                translate: 'CUSTOMERID',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'customerName',
                translate: 'CUSTOMERNAME',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'status',
                translate: 'STATUS',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'description',
                translate: 'DESCRIPTION',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'assignee',
                translate: 'ASSIGNEE',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'assigneeName',
                translate: 'ASSIGNEENAME',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'representativeName',
                translate: 'REPRESENTATIVENAME',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'qualityControlItemCount',
                translate: 'QUALITYCONTROLITEMCOUNT',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'qualityControlItemPlannedCount',
                translate: 'QUALITYCONTROLITEMCOUNTALL',
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



export class QualityControlData {

    qualityControlId: number;
    customerId: number;
    customerName: number;
    status: number;
    description: string;
    assignee: number;
    assigneeName: string;
    qualityControlItemCount: number;
    qualityControlItemPlannedCount: number;
    qualityControlItemOkCount: number;
    qualityControlItemNotOkCount: number;
    representative: number;
    representativeName: string;

    createdDate: Date;
    createdUser: number;
    createdUserText: string;
    updatedDate: Date;
    updatedUser: number;
    updatedUserText: string;

    constructor() {

    }
}

export class QualityControlFilter {

    qualityControlId: number;
    customerId: number;
    status: number;
    activeStatus: boolean;
    description: string;
    assignee: number;
    representative: number;
    createdDate: Date;
    createdDate2: Date;
    createdUser: number;
    updatedDate: Date;
    updatedDate2: Date;
    updatedUser: number;

    constructor() {

    }
}

export class BulkQualityComfirmationData {
   
    bulkQualityControlItemList: BulkQualityItemComfirmationData[];
    constructor() {

    }
}

export class BulkQualityItemComfirmationData {
    qualityControlItemId: number;
}


