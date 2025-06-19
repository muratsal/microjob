import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PermissionData, PermissionFilter } from "./permission.component";
import { QueryInfo } from "../../../shared/query-info";
import { JwtAuthService } from "../../../shared/services/auth/jwt-auth.service";
import { environment } from "environments/environment";

@Injectable({
    providedIn: 'root'
})
export class PermissionService {

    endPointBase = environment.apiURL + "/Permission/";

    constructor(private httpClient: HttpClient, private authService: JwtAuthService) { }

    save(data: PermissionData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "SavePermission" + "?session=" + this.authService.getToken(), data);
    }
    comfirmPermission(data: PermissionData, isComfirmed: boolean): Observable<object> {
        return this.httpClient.post(this.endPointBase + "ComfirmPermission" + "?isComfirmed=" + isComfirmed +"&session=" + this.authService.getToken(), data);
    }

    delete(data: PermissionData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "DeletePermission?session=" + this.authService.getToken(), data);
    }

    getData(filter: PermissionFilter, queryInfo: QueryInfo, columnInfos: any, isExport: boolean): Observable<any> {

        let filterData = {
            filter: filter,
            queryInfo: queryInfo,
            isExport: isExport,
            columnInfos: columnInfos
        }

        return this.httpClient.post(this.endPointBase + "GetPermissionList" + "?session=" + this.authService.getToken(), filterData);
    }

}
