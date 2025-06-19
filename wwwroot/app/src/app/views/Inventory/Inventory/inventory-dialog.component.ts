import { MatDialogRef } from "@angular/material/dialog";
import { InventoryData } from "./inventory.component";
import { Component, Inject } from "@angular/core";
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormControl } from '@angular/forms';
import { InventoryService } from './inventory.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { NavigationService } from '../../../shared/services/navigation.service'
import { ParameterFilter } from "../../System/Parameter/parameter.component";
import { ParameterService } from "../../System/Parameter/parameter.service";
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from "@angular/material/core";
import { MAT_MOMENT_DATE_ADAPTER_OPTIONS, MAT_MOMENT_DATE_FORMATS, MomentDateAdapter } from "@angular/material-moment-adapter";
import { environment } from "../../../../environments/environment";
import { CompanyFilter } from "../../System/Company/company.component";
import { CompanyService } from "../../System/Company/company.service";
import { Pager, QueryInfo } from "../../../shared/query-info";
import { InventoryTransactionData, InventoryTransactionFilter } from "../InventoryTransaction/inventorytransaction.component";
import { PageEvent } from "@angular/material/paginator";
import { InventoryTransactionService } from "../InventoryTransaction/inventorytransaction.service";
import { MatTableDataSource } from "@angular/material/table";
import { InventoryTransactionDialogComponent } from "../InventoryTransaction/inventorytransaction-dialog.component";
import { ComfirmDialogComponent } from "../../../shared/components/comfirm/comfirm.component";

@Component({
    selector: "inventory-dialog",
    templateUrl: "./inventory-dialog.component.html",
    providers: [
        { provide: MAT_DATE_LOCALE, useValue: 'tr-TR' },
        { provide: MAT_MOMENT_DATE_ADAPTER_OPTIONS, useValue: { useUtc: true } },
        { provide: MAT_DATE_FORMATS, useValue: MAT_MOMENT_DATE_FORMATS },
        { provide: DateAdapter, useClass: MomentDateAdapter, deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS] },
    ]
})
export class InventoryDialogComponent {

    row: InventoryData;
    isNew: boolean;
    inventoryTypeList: any;
    environment: any;
    companyList: any;

    queryInfo: QueryInfo;
    showSpinner: boolean = false;
    displayedColumns: string[] = [];
    dataSource: any;
    filter: InventoryTransactionFilter;
    pageEvent: PageEvent;


    constructor(public dialogRef: MatDialogRef<InventoryDialogComponent>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private inventoryService: InventoryService,
        private translate: TranslateService,
        private parameterService: ParameterService,
        private companyService: CompanyService,
        public dialog: MatDialog,
        private inventoryTransactionService: InventoryTransactionService,
        private navigationService: NavigationService,
        private snackbar: MatSnackBar) {
        this.row = data.row;
        this.isNew = data.isNew;
        this.environment = environment;
    }

    ngOnInit() {
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

        this.filter = new InventoryTransactionFilter();
        this.queryInfo = new QueryInfo();
        this.queryInfo.pager = new Pager();
        this.queryInfo.pager.currentPage = 0;
        this.queryInfo.pager.pageSize = 10;
        this.queryInfo.pager.totalCount = 0;

        this.displayedColumns = this.getDataConf().filter((x) => x.showColumn == true).map((c) => c.prop)
        this.getData(false);

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

        this.inventoryService.save(this.row).subscribe((response: any) => {

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
    }

    onFileSelected(event) {
        const file: File = event.target.files[0];

        if (file) {

            this.inventoryService.uploadInventoryPicture(file).subscribe((response: any) => {
                if (response.isSuccess) {
                    this.row.picture = response.data;
                }
            });
        }
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
        this.filter.inventoryId = this.row.inventoryId;
        this.showSpinner = true;
        let columnInfo = null;
        if (isExport) {

            columnInfo = this.getDataConf().filter(x => x.isExport == true)
                .map((x) => ({
                    prop: x.prop,
                    localText: this.translate.instant('INVENTORYTRANSACTION.' + x.translate)
                }));

        }


        this.inventoryTransactionService.getData(this.filter, this.queryInfo, columnInfo, isExport)
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

        let emptyData = new InventoryTransactionData();

        emptyData.inventoryTransId = 0;
        emptyData.inventoryId = this.row.inventoryId;
        emptyData.transDate = new Date();
        emptyData.employeeId = this.row.lastTransaction.transType == 3 ? this.row.lastTransaction.employeeId : 0;
        emptyData.transType = this.row.lastTransaction.transType == 1 ? 3 :
            (this.row.lastTransaction.transType == 3 ? 4 : 3);
        emptyData.note = '';
        emptyData.createdDate = new Date();
        emptyData.createdUser = 0;
        emptyData.updatedDate = new Date();
        emptyData.updatedUser = 0;

        this.openDialog(emptyData, true);
    }

    editItemDialog(row: InventoryTransactionData) {
        this.openDialog(row, false);
    }

    openDialog(row: InventoryTransactionData, isNew: boolean) {

        this.dialog.open(InventoryTransactionDialogComponent, {
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
                    this.deleteTransaction(response.data);
                }

                else if (response.operation == "save") {
                    this.getData(false);
                }

            });
    }
    deleteTransaction(data: InventoryTransactionData) {

        let deleteRow = data;
        this.dialog.open(ComfirmDialogComponent, {
            hasBackdrop: true
        }).afterClosed()
            .subscribe(response => {
                if (response.comfirm == true) {
                    this.inventoryTransactionService.delete(deleteRow).subscribe((response: any) => {
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

    downloadData() {
        this.getData(true);
    }

    getDataConf() {
        return [

            {
                prop: 'Actions',
                translate: 'ACTIONS',
                showColumn: false,
                isExport: false,
                localText: ''
            },
            {
                prop: 'inventoryTransId',
                translate: 'INVENTORYTRANSID',
                showColumn: true,
                isExport: true,
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
                prop: 'inventorySerieNo',
                translate: 'INVENTORYSERIENO',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'transDate',
                translate: 'TRANSDATE',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'employeeId',
                translate: 'EMPLOYEEID',
                showColumn: false,
                isExport: true,
                localText: ''
            },
            {
                prop: 'employeeName',
                translate: 'EMPLOYEENAME',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'transType',
                translate: 'TRANSTYPE',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'note',
                translate: 'NOTE',
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

}