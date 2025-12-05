import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';
import {
  ApplicationConfig,
  EnvironmentProviders,
  Provider,
  inject,
  PLATFORM_ID,
  provideAppInitializer,
  provideZoneChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import {
  provideClientHydration,
  withEventReplay,
} from '@angular/platform-browser';
import {
  includeBearerTokenInterceptor,
  provideKeycloak,
} from 'keycloak-angular';
import Keycloak from 'keycloak-js';
import { appRoutes } from './app.routes';
import { SpaAuthConfig } from './core/models/spa-auth-config.model';
import { SPA_AUTH_CONFIG } from './core/services/spa-auth-config.service';

const removeTrailingSlash = (value: string) =>
  value.endsWith('/') ? value.slice(0, -1) : value;

export const DEFAULT_SPA_AUTH_CONFIG: SpaAuthConfig = {
  authServerUrl: 'http://localhost:8081/',
  realm: 'Test',
  clientId: 'todo-web',
  redirectUri: 'http://localhost:4200/',
  logoutRedirectUri: 'http://localhost:4200/',
  requireHttps: false,
};

const createSharedProviders = (
  spaConfig: SpaAuthConfig
): Array<Provider | EnvironmentProviders> => [
  provideRouter(appRoutes),
  provideClientHydration(withEventReplay()),
  provideZoneChangeDetection({ eventCoalescing: true }),
  provideHttpClient(withInterceptors([includeBearerTokenInterceptor])),
  {
    provide: SPA_AUTH_CONFIG,
    useValue: spaConfig,
  },
];

const createSilentCheckUri = (redirectUri: string) =>
  `${removeTrailingSlash(redirectUri)}/silent-check-sso.html`;

const createKeycloakConfig = (spaConfig: SpaAuthConfig) => ({
  url: spaConfig.authServerUrl,
  realm: spaConfig.realm,
  clientId: spaConfig.clientId,
});

export const createAppConfig = (
  spaConfig: SpaAuthConfig = DEFAULT_SPA_AUTH_CONFIG
): ApplicationConfig => {
  const silentCheckSsoRedirectUri = createSilentCheckUri(spaConfig.redirectUri);

  return {
    providers: [
      provideKeycloak({
        config: createKeycloakConfig(spaConfig),
      }),
      provideAppInitializer(() => {
        if (!isPlatformBrowser(inject(PLATFORM_ID))) {
          // Skip initialization on server
          return Promise.resolve(true);
        }
        const keycloak = inject(Keycloak);
        return keycloak.init({
          onLoad: 'check-sso',
          silentCheckSsoRedirectUri,
        });
      }),
      ...createSharedProviders(spaConfig),
    ],
  };
};

export const createServerAppConfig = (
  spaConfig: SpaAuthConfig = DEFAULT_SPA_AUTH_CONFIG
): ApplicationConfig => ({
  providers: [
    provideKeycloak({
      config: createKeycloakConfig(spaConfig),
    }),
    ...createSharedProviders(spaConfig),
  ],
});

export const appConfig = createAppConfig();
export const serverAppConfig = createServerAppConfig();
