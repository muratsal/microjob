import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MenuData, MenuFilter } from "./menu.component";
import { QueryInfo } from "../../../shared/query-info";
import { JwtAuthService } from "../../../shared/services/auth/jwt-auth.service";
import { environment } from "environments/environment";

@Injectable({
    providedIn: 'root'
})
export class MenuService {

    endPointBase = environment.apiURL + "/Menu/";

    constructor(private httpClient: HttpClient, private authService: JwtAuthService) { }

    save(data: MenuData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "SaveMenu" + "?session=" + this.authService.getToken(), data);
    }

    delete(data: MenuData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "DeleteMenu?session=" + this.authService.getToken(), data);
    }

    getData(filter: MenuFilter, queryInfo: QueryInfo, columnInfos: any, isExport: boolean): Observable<any> {

        let filterData = {
            filter: filter,
            queryInfo: queryInfo,
            isExport: isExport,
            columnInfos: columnInfos
        }

        return this.httpClient.post(this.endPointBase + "GetMenuList" + "?session=" + this.authService.getToken(), filterData);
    }

}
