import { Routes } from '@angular/router';

import { QualityControlComponent } from './QualityControl/qualitycontrol.component';
import { QualityControlDetailComponent } from './QualityControlDetail/qualitycontroldetail.component';
import { QualityControlItemComponent } from './QualityControlItem/qualitycontrolitem.component';
import { QualityControlItemImageComponent } from './QualityControlItemImage/qualitycontrolitemimage.component';


//@Component_Imports


export const QualityRoutes: Routes = [
    {
        path: '',
        children: [
            {
                path: 'qualitycontrol',
                component: QualityControlComponent,
                data: {
                    title: 'QUALITYCONTROL.QUALITYCONTROLMENU', breadcrumb: 'QUALITYCONTROL.QUALITYCONTROLMENU'
                }
            },
            {
                path: 'qualitycontrolitem',
                component: QualityControlItemComponent,
                data: {
                    title: 'QUALITYCONTROLITEM.QUALITYCONTROLITEMMENU', breadcrumb: 'QUALITYCONTROLITEM.QUALITYCONTROLITEMMENU'
                }
            },
            {
                path: 'qualitycontroldetail/:ccid',
                component: QualityControlDetailComponent,
                data: { title: 'QUALITYCONTROL.QUALITYCONTROLMENU', breadcrumb: 'QUALITYCONTROL.QUALITYCONTROLMENU' }
            },
            {
                path: 'qualitycontrolitemimage',
                component: QualityControlItemImageComponent,
                data: { title: 'QUALITYCONTROLITEMIMAGE.QUALITYCONTROLITEMIMAGEMENU', breadcrumb: 'QUALITYCONTROLITEMIMAGE.QUALITYCONTROLITEMIMAGEMENU' }
            },
            //@Component_Routes
        ]
    }
];
