import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { QualityControlItemData, QualityControlItemFilter } from "./qualitycontrolitem.component";
import { QueryInfo } from "../../../shared/query-info";
import { JwtAuthService } from "../../../shared/services/auth/jwt-auth.service";
import { environment } from "environments/environment";
import { QualityControlItemImageData } from '../QualityControlItemImage/qualitycontrolitemimage.component';

@Injectable({
    providedIn: 'root'
})
export class QualityControlItemService {

    endPointBase = environment.apiURL + "/QualityControlItem/";

    constructor(private httpClient: HttpClient, private authService: JwtAuthService) { }

    save(data: QualityControlItemData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "SaveQualityControlItem" + "?session=" + this.authService.getToken(), data);
    }

    delete(data: QualityControlItemData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "DeleteQualityControlItem?session=" + this.authService.getToken(), data);
    }

    getData(filter: QualityControlItemFilter, queryInfo: QueryInfo, columnInfos: any, isExport: boolean): Observable<any> {

        let filterData = {
            filter: filter,
            queryInfo: queryInfo,
            isExport: isExport,
            columnInfos: columnInfos
        }

        return this.httpClient.post(this.endPointBase + "GetQualityControlItemList" + "?session=" + this.authService.getToken(), filterData);
    }

    getQualityControlItemWithImagesById(ccoiId:number): Observable<any> {

        return this.httpClient.get(this.endPointBase + "GetQualityControlItemWithImagesById?ccoiId=" + ccoiId.toString() + "&session=" + this.authService.getToken());
    }
    getQualityControlItemWithImagesByOrderItemId(customerOrderItemId: number): Observable<any> {

        return this.httpClient.get(this.endPointBase + "GetQualityControlItemWithImagesByOrderItemId?customerOrderItemId=" + customerOrderItemId.toString() + "&session=" + this.authService.getToken());
    }

    
    bulkStatusQualityControlItem(qualityControlItemList: number[], status: number): Observable<any> {

        let postData = {
            qualityControlItemList: qualityControlItemList,
            status: status
        }

        return this.httpClient.post(this.endPointBase + "BulkStatusQualityControlItem" + "?session=" + this.authService.getToken(), postData);
    }

    

    bulkDelete(data: number[]): Observable<object> {
        return this.httpClient.post(this.endPointBase + "BulkDeleteQualityControlItem?session=" + this.authService.getToken(), data);
    }
}

export class QualityControlItemWithImageData {

    qualityControlItem: QualityControlItemData;
    qualityControlItemImages: QualityControlItemImageData[];

}
