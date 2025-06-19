import { Component, OnInit, ViewChild, Inject } from '@angular/core';
import { UserService } from './user.service';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA, MatDialogConfig } from '@angular/material/dialog';
import { UserDialogComponent } from './user-dialog.component';
import { QueryInfo, Pager } from '../../../shared/query-info';
import { TranslateService } from '@ngx-translate/core';
import { environment } from "environments/environment";
import { ComfirmDialogComponent } from "../../../shared/components/comfirm/comfirm.component";
import { MatSnackBar } from '@angular/material/snack-bar';
import { NavigationService } from '../../../shared/services/navigation.service'
import { map } from 'rxjs/operators';



@Component({
    selector: 'user-table',
    templateUrl: './user.component.html',
    styleUrls: ['./user.component.scss']
})
export class UserComponent implements OnInit {

    queryInfo: QueryInfo;
    showSpinner: boolean = false;
    displayedColumns: string[] = [];
    dataSource: any;
    filter: UserFilter;
    pageEvent: PageEvent;
    data: Array<any>;
    jokes: any;
    currentJoke: any;


    constructor(private userService: UserService,
        public dialog: MatDialog,
        public translate: TranslateService,
        private snackbar: MatSnackBar,
        private navigationService: NavigationService) { }

    ngOnInit() {

        this.filter = new UserFilter();
        this.queryInfo = new QueryInfo();
        this.queryInfo.pager = new Pager();
        this.queryInfo.pager.currentPage = 0;
        this.queryInfo.pager.pageSize = 10;
        this.queryInfo.pager.totalCount = 0;
        this.data = [];

        this.displayedColumns = this.getDataConf().filter((x) => x.showColumn == true).map((c) => c.prop)
        this.getData(false);
    }

    ngAfterViewInit() {

    }

    clearFilter() {
        this.filter = new UserFilter();
    }

    filterData() {
        this.queryInfo.pager.currentPage = 0;
        this.getData(false);
    }

    downloadData() {
        this.getData(true);
    }

    sortData($event) {
        console.log($event);

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
                    localText: this.translate.instant('USER.' + x.translate)
                }));

        }


        this.userService.getData(this.filter, this.queryInfo, columnInfo, isExport)
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

                }

            );
    }

    newItemDialog() {

        let emptyData = new UserData();

        emptyData.userId = 0;
        emptyData.userName = '';
        emptyData.email = '';
        emptyData.password = '';
        emptyData.firstName = '';
        emptyData.lastName = '';
        emptyData.isActive = false;
        emptyData.lastLoginDate = new Date();
        emptyData.lastLoginIp = '';
        emptyData.createdDate = new Date();
        emptyData.createdUser = 0;
        emptyData.createdUserText = '';
        emptyData.updatedDate = new Date();
        emptyData.updatedUser = 0;
        emptyData.updatedUserText = '';

        this.openDialog(emptyData, true);
    }

    editItemDialog(row: UserData) {
        this.openDialog(row, false);
    }

    openDialog(row: UserData, isNew: boolean) {

        this.dialog.open(UserDialogComponent, {
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


    delete(data: UserData) {

        let deleteRow = data;
        this.dialog.open(ComfirmDialogComponent, {
            hasBackdrop: true
        }).afterClosed()
            .subscribe(response => {
                if (response.comfirm == true) {
                    this.userService.delete(deleteRow).subscribe((response: any) => {

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

                        }
                    );
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

    userChange($event, row, field) {
        row[field] = $event;
    }

    //doFilter() {
    //    if ((typeof this.currentJoke) == "string")
    //        this.userService.getUserListForCombo(this.currentJoke, 0).subscribe((response: any) => {
    //            this.navigationService.sessionControl(response);
    //            this.data = response.data;
    //        });

    //}

    //autoCompleteDisplay(item: any): string {
    //    if (item == undefined) { return }
    //    return item.value + ' - ' + item.displayText;
    //}

    //filter2(values) {
    //    return values.filter(joke =>
    //        // used 'includes' here for demo, you'd want to probably use 'indexOf'
    //        joke.joke.toLowerCase().includes(this.currentJoke))
    //}




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
                prop: 'userId',
                translate: 'USERID',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'userName',
                translate: 'USERNAME',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'password',
                translate: 'PASSWORD',
                showColumn: false,
                isExport: false,
                localText: ''
            },
            {
                prop: 'email',
                translate: 'EMAIL',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'firstName',
                translate: 'FIRSTNAME',
                showColumn: true,
                isExport: true,
                localText: ''
            },
            {
                prop: 'lastName',
                translate: 'LASTNAME',
                showColumn: true,
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
                prop: 'userType',
                translate: 'USERTYPE',
                showColumn: false,
                isExport: false,
                localText: ''
            },
            {
                prop: 'accesFromOutside',
                translate: 'ACCESFROMOUTSIDE',
                showColumn: false,
                isExport: false,
                localText: ''
            },
            {
                prop: 'updatedDate',
                translate: 'UPDATEDDATE',
                showColumn: true,
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



export class UserData {

    userId: number;
    userName: string;
    email: string;
    password: string;
    firstName: string;
    lastName: string;
    isActive: boolean;
    lastLoginDate: Date;
    lastLoginIp: string;
    profileImage: string;
    userType: number;
    customerId: number;
    createdDate: Date;
    createdUser: number;
    createdUserText: string;
    updatedDate: Date;
    updatedUser: number;
    updatedUserText: string;
    accesFromOutside: boolean;
    language: number;

    constructor() {

    }
}

export class UserFilter {

    userId: number;
    userName: string;
    email: string;
    password: string;
    firstName: string;
    lastName: string;
    isActive: boolean;
    lastLoginDate: Date;
    lastLoginDate2: Date;
    lastLoginIp: string;
    userType: number;
    customerId: number;
    createdDate: Date;
    createdDate2: Date;
    createdUser: number;
    updatedDate: Date;
    updatedDate2: Date;
    updatedUser: number;
    accesFromOutside: boolean;
    language: number;

    constructor() {

    }
}


