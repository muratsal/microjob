import { Component, OnInit, OnDestroy, Input, Output, EventEmitter } from '@angular/core';
import { FastAutoCompleteService } from './fast-autocomplete.service';
import { FormControl } from '@angular/forms';
import 'rxjs/add/operator/debounceTime';


@Component({
    selector: 'fast-autocomplete',
    templateUrl: './fast-autocomplete.component.html',
    styleUrls: ['./fast-autocomplete.component.scss'],
})
export class FastAutoCompleteComponent implements OnInit, OnDestroy {

    @Input()
    placeholder: string;
    @Input()
    endpoint: string;

    @Input()
    initValue: number;

    @Input()
    isRequired: boolean;

    @Input()
    showRemove: boolean;


  

    @Output()
    onChange: EventEmitter<number> = new EventEmitter<number>();


    currentValue: any;
    dataList: any;
    autoFormControl: any;


    constructor(
        private fastAutoCompleteService: FastAutoCompleteService,
    ) {
        this.autoFormControl = new FormControl();
    }

    ngOnInit() {

        if (this.initValue && this.initValue != 0) {
            this.fastAutoCompleteService.getFilterDataList("", parseInt(this.initValue.toString()), this.endpoint).subscribe((response: any) => {
                //this.navigationService.sessionControl(response);
                this.dataList = response.data;
                if (this.dataList.length > 0)
                    this.autoFormControl.setValue(this.dataList[0]);
            });
        }

        this.autoFormControl.valueChanges.debounceTime(500).subscribe(value => {
            if (typeof value == 'string') {
                this.fastAutoCompleteService.getFilterDataList(value, 0, this.endpoint).subscribe((response: any) => {
                    //this.navigationService.sessionControl(response);
                    this.dataList = response.data;

                });
            }
            else {
                this.onChange.emit(value.value);
            }
        });
    }
  
    ngOnChanges(value:any) {

        if (value.initValue &&  value.initValue.currentValue != value.initValue.previousValue) {
            this.fastAutoCompleteService.getFilterDataList("", parseInt(this.initValue.toString()), this.endpoint).subscribe((response: any) => {
                //this.navigationService.sessionControl(response);
                this.dataList = response.data;
                if (this.dataList.length > 0)
                    this.autoFormControl.setValue(this.dataList[0]);
            });
        }
    }

    ngOnDestroy() {

    }

    doFilter() {
        if ((typeof this.currentValue) == "string")
            this.fastAutoCompleteService.getFilterDataList(this.currentValue, 0, this.endpoint).subscribe((response: any) => {
                //this.navigationService.sessionControl(response);
                this.dataList = response.data;
            });

    }

    autoCompleteDisplay(item: any): string {
        if (item == undefined) { return }
        return item.displayText;
    }

    onClickRemoveValue() {
        this.autoFormControl.setValue(undefined);
        this.onChange.emit(undefined);
    }


}
