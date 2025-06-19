import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { QualityControlData, QualityControlFilter } from "./qualitycontrol.component";
import { QueryInfo } from "../../../shared/query-info";
import { JwtAuthService } from "../../../shared/services/auth/jwt-auth.service";
import { environment } from "environments/environment";

@Injectable({
    providedIn: 'root'
})
export class QualityControlService {

    endPointBase = environment.apiURL + "/QualityControl/";

    constructor(private httpClient: HttpClient, private authService: JwtAuthService) { }

    save(data: QualityControlData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "SaveQualityControl" + "?session=" + this.authService.getToken(), data);
    }

    delete(data: QualityControlData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "DeleteQualityControl?session=" + this.authService.getToken(), data);
    }

    getData(filter: QualityControlFilter, queryInfo: QueryInfo, columnInfos: any, isExport: boolean): Observable<any> {

        let filterData = {
            filter: filter,
            queryInfo: queryInfo,
            isExport: isExport,
            columnInfos: columnInfos
        }

        return this.httpClient.post(this.endPointBase + "GetQualityControlList" + "?session=" + this.authService.getToken(), filterData);
    }

}
