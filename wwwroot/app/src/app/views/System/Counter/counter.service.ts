import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CounterData, CounterFilter } from "./counter.component";
import { QueryInfo } from "../../../shared/query-info";
import { JwtAuthService } from "../../../shared/services/auth/jwt-auth.service";
import { environment } from "environments/environment";

@Injectable({
    providedIn: 'root'
})
export class CounterService {

    endPointBase = environment.apiURL + "/Counter/";

    constructor(private httpClient: HttpClient, private authService: JwtAuthService) { }

    save(data: CounterData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "SaveCounter" + "?session=" + this.authService.getToken(), data);
    }

    delete(data: CounterData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "DeleteCounter?session=" + this.authService.getToken(), data);
    }

    getData(filter: CounterFilter, queryInfo: QueryInfo, columnInfos: any, isExport: boolean): Observable<any> {

        let filterData = {
            filter: filter,
            queryInfo: queryInfo,
            isExport: isExport,
            columnInfos: columnInfos
        }

        return this.httpClient.post(this.endPointBase + "GetCounterList" + "?session=" + this.authService.getToken(), filterData);
    }

}
