import { HttpClient } from '@angular/common/http';
import { Injectable, InjectionToken, Inject, Optional } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { SpaAuthConfig } from '../models/spa-auth-config.model';

export const SPA_AUTH_CONFIG = new InjectionToken<SpaAuthConfig>(
  'SPA_AUTH_CONFIG (preloaded Keycloak SPA configuration)'
);

@Injectable({ providedIn: 'root' })
export class SpaAuthConfigService {
  private cachedConfig?: SpaAuthConfig;

  constructor(
    private readonly http: HttpClient,
    @Optional() @Inject(SPA_AUTH_CONFIG) private readonly preloadedConfig: SpaAuthConfig | null = null
  ) {
    this.cachedConfig = preloadedConfig ?? undefined;
  }

  async loadConfig(): Promise<SpaAuthConfig> {
    if (this.cachedConfig) {
      return this.cachedConfig;
    }

    const config = await firstValueFrom(
      this.http.get<SpaAuthConfig>('/api/identity/spa-config')
    );

    this.cachedConfig = config;
    return config;
  }
}
