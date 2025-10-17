import { Route } from '@angular/router';
import { TodoListComponent } from './todo-list/todo-list.component';

export const angularTodosFeatureRoutes: Route[] = [
  { path: '', component: TodoListComponent },
];
