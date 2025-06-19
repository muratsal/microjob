import { Routes } from '@angular/router';

import { InventoryComponent } from './Inventory/inventory.component';
import { InventoryTransactionComponent } from './InventoryTransaction/inventorytransaction.component';


//@Component_Imports


export const InventoryRoutes: Routes = [
    {
        path: '',
        children: [
            {
                path: 'inventory',
                component: InventoryComponent,
                data: { title: 'INVENTORY.INVENTORYMENU', breadcrumb: 'INVENTORY.INVENTORYMENU' }
            },
            {
                path: 'inventorytransaction',
                component: InventoryTransactionComponent,
                data: { title: 'INVENTORYTRANSACTION.INVENTORYTRANSACTIONMENU', breadcrumb: 'INVENTORYTRANSACTION.INVENTORYTRANSACTIONMENU' }
            }
         
            //@Component_Routes
        ]
    }
];
