import { Route } from '@angular/router';

export const appRoutes: Route[] = [
  {
    path: '',
    loadComponent: () =>
      import('@cheese-grater/angular/todos/feature').then(
        (m) => m.LayoutComponent
      ),
    loadChildren: () =>
      import('@cheese-grater/angular/todos/feature').then(
        (m) => m.angularTodosFeatureRoutes
      ),
  },
  {
    path: 'auth-redirect',
    loadComponent: () =>
      import('@cheese-grater/angular/todos/feature').then(
        (m) => m.AuthRedirectComponent
      ),
  },
];
