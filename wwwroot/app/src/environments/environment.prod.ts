import { config } from "config";

export const environment = {
    production: true,
    apiURL: location.href.indexOf("erp.asrra.com") > 0 ? 'http://erp.asrra.com/api' : 'http://192.168.1.9:9999/api',
    imagePath: location.href.indexOf("erp.asrra.com") > 0 ? 'http://erp.asrra.com/' : 'http://192.168.1.9:9999/',
    loginPage: config.loginPage
};

//export const environment = {
//    production: true,
//    apiURL: 'http://192.168.1.12:9999/api',
//    imagePath:  'http://192.168.1.12:9999/',
//    loginPage: config.loginPage
//};