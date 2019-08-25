import { MemberDetailsResolver } from './resolvers/member-details.resolver';
import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './guards/auth.guard';
import { MemberDetailsComponent } from './members/member-details/member-details.component';
import { MemberListResolver } from './resolvers/member-list.resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './resolvers/member-edit.resolver';
import { PreventUnsavedChangesGuard } from './guards/prevent-unsaved-changes.guard';
import { ListResolver } from './resolvers/list.resolver';
import { MessagesResolver } from './resolvers/messages.resolver';

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
            {path: 'member/edit', component: MemberEditComponent,
             resolve: {user: MemberEditResolver}, canDeactivate: [ PreventUnsavedChangesGuard ]},
            {path: 'messages', component: MessagesComponent, resolve: { messages: MessagesResolver}},
            {path: 'lists', component: ListsComponent, resolve: {users: ListResolver}},
        ]
    },
    {path: '**', redirectTo: '', pathMatch: 'full'},
];
