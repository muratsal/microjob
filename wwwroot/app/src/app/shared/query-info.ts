export class QueryInfo {
    orderby: string;
    pager: Pager;
}

 export class Pager {
    totalCount: number;
    pageSize: number;
    currentPage: number;
}