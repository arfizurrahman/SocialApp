import { MemberDetailsResolver } from './resolvers/member-details.resolver';
import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './guards/auth.guard';
import { MemberDetailsComponent } from './members/member-details/member-details.component';
import { MemberListResolver } from './resolvers/member-list.resolver';

export const appRoutes: Routes = [
    {path: '', component: HomeComponent},
    {
        path: '', // path: 'dummy' => localhost:5000/dummymessages
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            {path: 'members', component: MemberListComponent,
            resolve: {users: MemberListResolver}},
            {path: 'members/:id', component: MemberDetailsComponent,
             resolve: {user: MemberDetailsResolver}},
            {path: 'messages', component: MessagesComponent},
            {path: 'lists', component: ListsComponent},
        ]
    },
    {path: '**', redirectTo: '', pathMatch: 'full'},
];