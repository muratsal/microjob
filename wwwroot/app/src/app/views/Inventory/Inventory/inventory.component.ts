import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { InventoryService } from './inventory.service';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA, MatDialogConfig } from '@angular/material/dialog';
import { InventoryDialogComponent } from './inventory-dialog.component';
import { QueryInfo, Pager } from '../../../shared/query-info';
import { TranslateService } from '@ngx-translate/core';
import { environment } from "environments/environment";
import { ComfirmDialogComponent } from "../../../shared/components/comfirm/comfirm.component";
import { MatSnackBar } from '@angular/material/snack-bar';
import { NavigationService } from '../../../shared/services/navigation.service'
import { InventoryTransactionData } from '../InventoryTransaction/inventorytransaction.component';
import { ParameterFilter } from '../../System/Parameter/parameter.component';
import { ParameterService } from '../../System/Parameter/parameter.service';
import { CompanyFilter } from '../../System/Company/company.component';
import { CompanyService } from '../../System/Company/company.service';



@Component({
    selector: 'inventory-table',
    templateUrl: './inventory.component.html',
    styleUrls: ['./inventory.component.scss']
})
export class InventoryComponent implements OnInit {

    queryInfo: QueryInfo;
    showSpinner: boolean = false;
    displayedColumns: string[] = [];
    dataSource: any;
    filter: InventoryFilter;
    pageEvent: PageEvent;
    environment: any;
    inventoryTypeList: any;
    companyList: any;

    constructor(private inventoryService: InventoryService,
        public dialog: MatDialog,
        public translate: TranslateService,
        public parameterService: ParameterService,
        public companyService: CompanyService,
        private snackbar: MatSnackBar,
        private navigationService: NavigationService) { }

    ngOnInit() {
        this.environment = environment;
        this.filter = new InventoryFilter();
        this.queryInfo = new QueryInfo();
        this.queryInfo.pager = new Pager();
        this.queryInfo.pager.currentPage = 0;
        this.queryInfo.pager.pageSize = 10;
        this.queryInfo.pager.totalCount = 0;

        this.displayedColumns = this.getDataConf().filter((x) => x.showColumn == true).map((c) => c.prop)
        this.getData(false);
    }

    ngAfterViewInit() {
        let inventoryTypeParamFilter = new ParameterFilter();
        inventoryTypeParamFilter.paramType = 3;

        this.parameterService.
            getData(inventoryTypeParamFilter, null, null, false)
            .subscribe((response: any) => {
                this.inventoryTypeList = response.data;
            });

        let companyFilter = new CompanyFilter();

        this.companyService.
            getData(companyFilter, null, null, false)
            .subscribe((response: any) => {
                this.companyList = response.data;
            });
    }

    clearFilter() {
        this.filter = new InventoryFilter();
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
                    localText: this.translate.instant('INVENTORY.' + x.translate)
                }));

        }


        this.inventoryService.getData(this.filter, this.queryInfo, columnInfo, isExport)
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

        let emptyData = new InventoryData();

        emptyData.inventoryId = 0;
        emptyData.inventoryName = '';
        emptyData.serieNo = '';
        emptyData.model = '';
        emptyData.inventoryType = 0;
        emptyData.isActive = false;
        emptyData.picture = '';
        emptyData.description = '';
        emptyData.note = '';
        emptyData.companyId = 0;
        emptyData.inventoryDate = new Date();
        emptyData.createdDate = new Date();
        emptyData.createdUser = 0;
        emptyData.updatedDate = new Date();
        emptyData.updatedUser = 0;

        this.openDialog(emptyData, true);
    }

    editItemDialog(row: InventoryData) {
        this.openDialog(row, false);
    }

    openDialog(row: InventoryData, isNew: boolean) {

        this.dialog.open(InventoryDialogComponent, {
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


    delete(data: InventoryData) {

        let deleteRow = data;
        this.dialog.open(ComfirmDialogComponent, {
            hasBackdrop: true
        }).afterClosed()
            .subscribe(response => {
                if (response.comfirm == true) {
                    this.inventoryService.delete(deleteRow).subscribe((response: any) => {
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
                prop: 'inventoryId',
                translate: 'INVENTORYID',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'inventoryType',
                translate: 'INVENTORYTYPE',
                showColumn: true,
                isExport: false,
                localText: ''
            },
            {
                prop: 'inventoryTypeText',
                translate: 'INVENTORYTYPE',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'inventoryName',
                translate: 'INVENTORYNAME',
                showColumn: false,
                isExport: false,
                localText: ''
            },
            {
                prop: 'serieNo',
                translate: 'SERIENO',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'model',
                translate: 'MODEL',
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
                prop: 'transTypeText',
                translate: 'TRANSTYPE',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'transactionEmployeeName',
                translate: 'EMPLOYEENAME',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'isActive',
                translate: 'ISACTIVE',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'picture',
                translate: 'PICTURE',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'description',
                translate: 'DESCRIPTION',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'note',
                translate: 'NOTE',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'companyId',
                translate: 'COMPANYID',
                showColumn: false,
                isExport: false,
                localText: ''
            },
            {
                prop: 'companyName',
                translate: 'COMPANYNAME',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'inventoryDate',
                translate: 'INVENTORYDATE',
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
                isExport: false,
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
                isExport: false,
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
}



export class InventoryData {

    inventoryId: number;
    inventoryName: string;
    serieNo: string;
    model: string;
    inventoryType: number;
    isActive: boolean;
    picture: string;
    description: string;
    note: string;
    companyId: number;
    companyName: string;
    inventoryDate: Date;
    transactionEmployeeName: string;
    transType: number; 
    transTypeText: number; 
    lastTransaction: InventoryTransactionData;
    createdDate: Date;
    createdUser: number;
    createdUserText: string;
    updatedDate: Date;
    updatedUser: number;
    updatedUserText: string;

    constructor() {

    }
}

export class InventoryFilter {

    inventoryId: number;
    inventoryName: string;
    serieNo: string;
    model: string;
    inventoryType: number;
    isActive: boolean;
    picture: string;
    description: string;
    note: string;
    companyId: number;
    inventoryDate: Date;
    inventoryDate2: Date
    createdDate: Date;
    createdDate2: Date;
    createdUser: number;
    updatedDate: Date;
    updatedDate2: Date;
    updatedUser: number;

    constructor() {

    }
}


