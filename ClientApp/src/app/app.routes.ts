import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { NotFoundComponent } from './shared/components/errors/not-found/not-found.component';
import { PlayComponent } from './play/play.component';
import { authorizationGuard } from './shared/guards/authorization.guard';

export const routes: Routes = [
    {path: '', component: HomeComponent},
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [authorizationGuard],
        children: [
            {path: 'play', component: PlayComponent}
        ]
    },
    {path: 'account', loadChildren: () => import('./account/account.routes').then(r => r.ACCOUNT_ROUTES)},
    {path: 'test', component: NotFoundComponent},
    {path: 'not-found', component: NotFoundComponent},
    {path: '**', component: NotFoundComponent, pathMatch: 'full'}
];