import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { InventoryData, InventoryFilter } from "./inventory.component";
import { QueryInfo } from "../../../shared/query-info";
import { JwtAuthService } from "../../../shared/services/auth/jwt-auth.service";
import { environment } from "environments/environment";

@Injectable({
    providedIn: 'root'
})
export class InventoryService {

    endPointBase = environment.apiURL + "/Inventory/";

    constructor(private httpClient: HttpClient, private authService: JwtAuthService) { }

    save(data: InventoryData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "SaveInventory" + "?session=" + this.authService.getToken(), data);
    }

    delete(data: InventoryData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "DeleteInventory?session=" + this.authService.getToken(), data);
    }

    getData(filter: InventoryFilter, queryInfo: QueryInfo, columnInfos: any, isExport: boolean): Observable<any> {

        let filterData = {
            filter: filter,
            queryInfo: queryInfo,
            isExport: isExport,
            columnInfos: columnInfos
        }

        return this.httpClient.post(this.endPointBase + "GetInventoryList" + "?session=" + this.authService.getToken(), filterData);
    }

    uploadInventoryPicture(file: File): Observable<object> {

        const formData = new FormData();
        formData.append("imageName", file);

        return this.httpClient.post(this.endPointBase + "UploadInventoryPicture"+"?session=" + this.authService.getToken(), formData);
    }

}
