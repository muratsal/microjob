import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { InventoryTransactionData, InventoryTransactionFilter } from "./inventorytransaction.component";
import { QueryInfo } from "../../../shared/query-info";
import { JwtAuthService } from "../../../shared/services/auth/jwt-auth.service";
import { environment } from "environments/environment";

@Injectable({
    providedIn: 'root'
})
export class InventoryTransactionService {

    endPointBase = environment.apiURL + "/InventoryTransaction/";

    constructor(private httpClient: HttpClient, private authService: JwtAuthService) { }

    save(data: InventoryTransactionData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "SaveInventoryTransaction" + "?session=" + this.authService.getToken(), data);
    }

    delete(data: InventoryTransactionData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "DeleteInventoryTransaction?session=" + this.authService.getToken(), data);
    }

    getData(filter: InventoryTransactionFilter, queryInfo: QueryInfo, columnInfos: any, isExport: boolean): Observable<any> {

        let filterData = {
            filter: filter,
            queryInfo: queryInfo,
            isExport: isExport,
            columnInfos: columnInfos
        }

        return this.httpClient.post(this.endPointBase + "GetInventoryTransactionList" + "?session=" + this.authService.getToken(), filterData);
    }

}
