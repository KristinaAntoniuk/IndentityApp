import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { NotFoundComponent } from './shared/components/errors/not-found/not-found.component';

export const routes: Routes = [
    {path: '', component: HomeComponent},
    {path: 'account', loadChildren: () => import('./account/account.routes').then(r => r.ACCOUNT_ROUTES)},
    {path: 'test', component: NotFoundComponent},
    {path: 'not-found', component: NotFoundComponent},
    {path: '**', component: NotFoundComponent, pathMatch: 'full'}
];