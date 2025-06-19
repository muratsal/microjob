import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PermissionLogData, PermissionLogFilter } from "./permissionlog.component";
import { QueryInfo } from "../../../shared/query-info";
import { JwtAuthService } from "../../../shared/services/auth/jwt-auth.service";
import { environment } from "environments/environment";

@Injectable({
    providedIn: 'root'
})
export class PermissionLogService {

    endPointBase = environment.apiURL + "/PermissionLog/";

    constructor(private httpClient: HttpClient, private authService: JwtAuthService) { }

    save(data: PermissionLogData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "SavePermissionLog" + "?session=" + this.authService.getToken(), data);
    }

    delete(data: PermissionLogData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "DeletePermissionLog?session=" + this.authService.getToken(), data);
    }

    getData(filter: PermissionLogFilter, queryInfo: QueryInfo, columnInfos: any, isExport: boolean): Observable<any> {

        let filterData = {
            filter: filter,
            queryInfo: queryInfo,
            isExport: isExport,
            columnInfos: columnInfos
        }

        return this.httpClient.post(this.endPointBase + "GetPermissionLogList" + "?session=" + this.authService.getToken(), filterData);
    }

}
