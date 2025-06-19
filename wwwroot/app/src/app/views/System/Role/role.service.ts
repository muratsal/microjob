import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RoleData, RoleFilter } from "./role.component";
import { QueryInfo } from "../../../shared/query-info";
import { JwtAuthService } from "../../../shared/services/auth/jwt-auth.service";
import { environment } from "environments/environment";
import { RoleAuthInfo } from "./role-dialog.component";

@Injectable({
    providedIn: 'root'
})
export class RoleService {

    endPointBase = environment.apiURL + "/Role/";

    constructor(private httpClient: HttpClient, private authService: JwtAuthService) { }

    save(data: RoleData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "SaveRole" + "?session=" + this.authService.getToken(), data);
    }

    saveWithAuths(data: RoleData, authData: RoleAuthInfo[]): Observable<object> {

        let roleWithAuth = {
            role: data,
            authInfos: authData
        }

        return this.httpClient.post(this.endPointBase + "SaveRoleWithAuths" + "?session=" + this.authService.getToken(), roleWithAuth);
    }

    delete(data: RoleData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "DeleteRole?session=" + this.authService.getToken(), data);
    }

    getData(filter: RoleFilter, queryInfo: QueryInfo, columnInfos: any, isExport: boolean): Observable<any> {

        let filterData = {
            filter: filter,
            queryInfo: queryInfo,
            isExport: isExport,
            columnInfos: columnInfos
        }

        return this.httpClient.post(this.endPointBase + "GetRoleList" + "?session=" + this.authService.getToken(), filterData);
    }

    getAuthInfo(id: number): Observable<any> {
        return this.httpClient.post(this.endPointBase + "GetAuthInfo" + "?session=" + this.authService.getToken(), { roleId: id });

    }

}
