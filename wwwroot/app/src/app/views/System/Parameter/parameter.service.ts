import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ParameterData, ParameterFilter } from "./parameter.component";
import { QueryInfo } from "../../../shared/query-info";
import { JwtAuthService } from "../../../shared/services/auth/jwt-auth.service";
import { environment } from "environments/environment";

@Injectable({
    providedIn: 'root'
})
export class ParameterService {

    endPointBase = environment.apiURL + "/Parameter/";

    constructor(private httpClient: HttpClient, private authService: JwtAuthService) { }

    save(data: ParameterData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "SaveParameter" + "?session=" + this.authService.getToken(), data);
    }

    delete(data: ParameterData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "DeleteParameter?session=" + this.authService.getToken(), data);
    }

    getData(filter: ParameterFilter, queryInfo: QueryInfo, columnInfos: any, isExport: boolean): Observable<any> {

        let filterData = {
            filter: filter,
            queryInfo: queryInfo,
            isExport: isExport,
            columnInfos: columnInfos
        }

        return this.httpClient.post(this.endPointBase + "GetParameterList" + "?session=" + this.authService.getToken(), filterData);
    }

}
