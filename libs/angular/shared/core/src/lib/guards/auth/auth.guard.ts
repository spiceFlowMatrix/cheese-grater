import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import Keycloak from 'keycloak-js';

const mapResourceRoles = (
  resourceAccess: { [key: string]: { roles: string[] } } = {}
): Record<string, string[]> => {
  return Object.entries(resourceAccess).reduce<Record<string, string[]>>(
    (roles, [key, value]) => {
      roles[key] = value.roles;
      return roles;
    },
    {}
  );
};

/**
 * @description
 * The setup of this was copied from the `createAuthGuard` helper
 * function from `keycloak-angular`. Using `createAuthGuard` would
 * kept leading to `No provider found` errors in Jest tests even though
 * I had initialized Keycloak (from `keycloak-js) as a provider.
 * 
 * I was unable to solve the problem but it seems to be due to module resolution
 * issues caused due to `keycloak-angular` and `keycloak-js` using ESM while Jest
 * uses CJS
 */
export const authGuard: CanActivateFn = (route, state) => {
  const keycloak = inject(Keycloak);

  const authenticated = keycloak?.authenticated ?? false;
  const grantedRoles = {
    resourceRoles: mapResourceRoles(keycloak?.resourceAccess),
    realmRoles: keycloak?.realmAccess?.roles ?? [],
  };
  const requiredRole = route.data['requiredRoles'];

  const hasRequiredRole = (roles: string[]): boolean =>
    roles.some((role) =>
      Object.values(grantedRoles.resourceRoles).some((grantedResourceRoles) =>
        grantedResourceRoles.includes(role)
      )
    );

  if (authenticated && (!requiredRole || hasRequiredRole(requiredRole))) {
    return true;
  }

  const router = inject(Router);
  return router.parseUrl('/auth-redirect');
};
