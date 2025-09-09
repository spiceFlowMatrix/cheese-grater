import { Route } from '@angular/router';

export const appRoutes: Route[] = [
  {
    path: '',
    loadComponent: () =>
      import('./core/layout/layout.component').then((m) => m.LayoutComponent),
    loadChildren: () =>
      import('./core/layout.routes').then((m) => m.layoutRoutes),
  },
  {
    path: 'auth-redirect',
    loadComponent: () =>
      import('./auth-redirect/auth-redirect.component').then(
        (m) => m.AuthRedirectComponent
      ),
  },
];
