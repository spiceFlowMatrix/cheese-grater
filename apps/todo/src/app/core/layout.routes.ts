import { Route } from '@angular/router';
import { TodoContainerComponent } from '../todo-container/todo-container.component';

export const layoutRoutes: Route[] = [
  {
    path: '',
    component: TodoContainerComponent,
  },
];
