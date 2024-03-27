import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { NotFoundComponent } from './shared/components/errors/not-found/not-found.component';
import { PlayComponent } from './play/play.component';
import { LoginComponent } from './account/login/login.component';

export const routes: Routes = [
    {path: '', redirectTo:'login' , pathMatch:'full'},
    {path: 'home', component: HomeComponent},
    {path: 'login', component: LoginComponent},
    {path: 'play', component: PlayComponent},
    {path: 'test', component: NotFoundComponent},
    {path: 'not-found', component: NotFoundComponent},
    {path: '**', component: NotFoundComponent, pathMatch: 'full'}
];