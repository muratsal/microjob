import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { QualityControlItemImageData, QualityControlItemImageFilter } from "./qualitycontrolitemimage.component";
import { QueryInfo } from "../../../shared/query-info";
import { JwtAuthService } from "../../../shared/services/auth/jwt-auth.service";
import { environment } from "environments/environment";

@Injectable({
    providedIn: 'root'
})
export class QualityControlItemImageService {

    endPointBase = environment.apiURL + "/QualityControlItemImage/";

    constructor(private httpClient: HttpClient, private authService: JwtAuthService) { }

    save(data: QualityControlItemImageData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "SaveQualityControlItemImage" + "?session=" + this.authService.getToken(), data);
    }

    delete(data: QualityControlItemImageData): Observable<object> {
        return this.httpClient.post(this.endPointBase + "DeleteQualityControlItemImage?session=" + this.authService.getToken(), data);
    }

    bulkDelete(data: number[]): Observable<object> {
        return this.httpClient.post(this.endPointBase + "BulkDeleteQualityControlItemImage?session=" + this.authService.getToken(), data);
    }

    getData(filter: QualityControlItemImageFilter, queryInfo: QueryInfo, columnInfos: any, isExport: boolean): Observable<any> {

        let filterData = {
            filter: filter,
            queryInfo: queryInfo,
            isExport: isExport,
            columnInfos: columnInfos
        }

        return this.httpClient.post(this.endPointBase + "GetQualityControlItemImageList" + "?session=" + this.authService.getToken(), filterData);
    }

    uploadImage(file: any, qualityControlItemId: number, description: string): Observable<object> {

        const formData = new FormData();
        formData.append("imageName", file);
        formData.append("qualityControlItemId", qualityControlItemId.toString());
        formData.append("description", description);

        return this.httpClient.post(this.endPointBase + "UploadImage?session=" + this.authService.getToken(), formData);
    }

}
