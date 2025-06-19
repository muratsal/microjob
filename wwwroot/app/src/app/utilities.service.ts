import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from "environments/environment";

@Injectable({
    providedIn: 'root'
})
export class UtilitiesService {

    dataURLToBlob(dataURL: string) {
        var BASE64_MARKER = ';base64,';
        if (dataURL.indexOf(BASE64_MARKER) == -1) {
            var parts = dataURL.split(',');
            var contentType = parts[0].split(':')[1];
            var raw = parts[1];

            return new Blob([raw], { type: contentType });
        }

        var parts = dataURL.split(BASE64_MARKER);
        var contentType = parts[0].split(':')[1];
        var raw = window.atob(parts[1]);
        var rawLength = raw.length;

        var uInt8Array = new Uint8Array(rawLength);

        for (var i = 0; i < rawLength; ++i) {
            uInt8Array[i] = raw.charCodeAt(i);
        }

        return new Blob([uInt8Array], { type: contentType });
    }

    resizeImage(image: HTMLImageElement,maxSize:number) {
        let canvas = document.createElement('canvas'),
            max_size = maxSize,
            width = image.width,
            height = image.height;
        if (width > height) {
            if (width > max_size) {
                height *= max_size / width;
                width = max_size;
            }
        } else {
            if (height > max_size) {
                width *= max_size / height;
                height = max_size;
            }
        }
        canvas.width = width;
        canvas.height = height;
        canvas.getContext('2d').drawImage(image, 0, 0, width, height);
        let dataUrl = canvas.toDataURL('image/jpeg');
        let resizedImage = this.dataURLToBlob(dataUrl);
        return resizedImage;
    }

    getImageThumbnail(path: string) {
        let regex = /[\\\/][\w\-]+\.jpg/gm;
        let matchString = path.match(regex);
        path = path.replace(regex,"\\Thumbnail"+ matchString)
        return path;
    }

    userHasAuth(auth: string) {
        return !!JSON.parse(localStorage.getItem("USER_INFO")).authInfos.find(x => x.authCode == auth);
    }

    getFilterCache(filterName : string ) {
        if (localStorage.getItem(filterName)) {
            return JSON.parse(localStorage.getItem(filterName));
        }
        else return null;
    }

    setFilterCache(filterName: string, filterObject: any) {
         localStorage.setItem(filterName, JSON.stringify(filterObject));
    }

    isMobileViewSelected() {
        if (localStorage.getItem("IS_MOBILE_VIEW") && localStorage.getItem("IS_MOBILE_VIEW") == "true")
            return true;
        else
            return false;
    }

    setMobileView(isMobileView: boolean) {
        localStorage.setItem("IS_MOBILE_VIEW", isMobileView.toString());
    }

    

}
