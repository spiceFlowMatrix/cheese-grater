import { TestBed } from '@angular/core/testing';
import { CanActivateFn, provideRouter, Router } from '@angular/router';
import { RouterTestingHarness } from '@angular/router/testing';

import { authGuard } from './auth.guard';
import Keycloak from 'keycloak-js/lib/keycloak';
import { Component } from '@angular/core';
import { faker } from '@faker-js/faker';

@Component({ template: '<h1>Protected Page</h1>' })
class ProtectedComponent {}
@Component({ template: '<h1>Role Protected Page</h1>' })
class RoleProtectedComponent {}
@Component({ template: '<h1>Redirect Page</h1>' })
class RedirectComponent {}

describe('authGuard', () => {
  let harness: RouterTestingHarness;
  let router: Router;
  const requiredRoles: string[] = faker.helpers.multiple(() =>
    faker.word.noun()
  );

  // Mock the Keycloak instance
  const mockKeycloak: Partial<Keycloak> = {
    authenticated: false,
    resourceAccess: {},
    realmAccess: { roles: [] },
  };

  const executeGuard: CanActivateFn = (...guardParameters) =>
    TestBed.runInInjectionContext(() => authGuard(...guardParameters));

  beforeEach(async () => {
    TestBed.configureTestingModule({
      providers: [
        {
          provide: Keycloak,
          useValue: mockKeycloak,
        },
        provideRouter([
          {
            path: 'protected',
            component: ProtectedComponent,
            canActivate: [authGuard],
          },
          {
            path: 'role-protected',
            component: RoleProtectedComponent,
            canActivate: [authGuard],
            data: {
              requiredRoles: requiredRoles,
            },
          },
          { path: 'auth-redirect', component: RedirectComponent },
        ]),
      ],
    }).compileComponents();

    harness = await RouterTestingHarness.create();
    router = TestBed.inject(Router);
  });

  it('should be created', () => {
    expect(executeGuard).toBeTruthy();
  });

  it('given NOT authenticated, when navigating to a protected resource, then navigation throws error and redirects to Redirect page', async () => {
    // Set the mock Keycloak state for this test
    mockKeycloak.authenticated = false;
    // mockKeycloak.resourceAccess = {
    //   'test-client': { roles: ['admin'] },
    // };
    // mockKeycloak.realmAccess = { roles: ['user'] };

    await expect(
      harness.navigateByUrl('/protected', ProtectedComponent)
    ).rejects.toThrow();

    expect(harness.routeNativeElement?.textContent).toContain('Redirect Page');
  });

  describe('given authenticated', () => {
    beforeEach(() => {
      mockKeycloak.authenticated = true;
    });

    it('when navigating to a protected route, then navigation should go through', async () => {
      // mockKeycloak.resourceAccess = {
      //   'test-client': { roles: ['admin'] },
      // };
      // mockKeycloak.realmAccess = { roles: ['user'] };

      await harness.navigateByUrl('/protected', ProtectedComponent);

      expect(harness.routeNativeElement?.textContent).toContain(
        'Protected Page'
      );
    });

    it('when navigating to a role-protected route, then navigation throws error', async () => {
      // mockKeycloak.resourceAccess = {
      //   'test-client': { roles: ['admin'] },
      // };
      // mockKeycloak.realmAccess = { roles: ['user'] };

      await expect(
        harness.navigateByUrl('/role-protected', RoleProtectedComponent)
      ).rejects.toThrow();
    });

    it('given all required role are assigned, when navigating to a role-protected route, then navigation goes through', async () => {
      mockKeycloak.resourceAccess = {
        'test-client': { roles: requiredRoles },
      };

      await harness.navigateByUrl('/role-protected', RoleProtectedComponent);

      expect(harness.routeNativeElement?.textContent).toContain(
        'Role Protected Page'
      );
    });
    it('given some required role are assigned, when navigating to a role-protected route, then navigation goes through', async () => {
      const start = Math.floor(Math.random() * requiredRoles.length);
      const end =
        start + Math.floor(Math.random() * (requiredRoles.length - start + 1));
      const roles = requiredRoles.slice(start, end);

      mockKeycloak.resourceAccess = {
        'test-client': { roles: roles },
      };

      await harness.navigateByUrl('/role-protected', RoleProtectedComponent);

      expect(harness.routeNativeElement?.textContent).toContain(
        'Role Protected Page'
      );
    });
  });
});
