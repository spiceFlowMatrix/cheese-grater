import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { createAppConfig, DEFAULT_SPA_AUTH_CONFIG } from './app/app.config';
import { SpaAuthConfig } from './app/core/models/spa-auth-config.model';

const loadSpaConfig = async (): Promise<SpaAuthConfig> => {
  try {
    const response = await fetch('/api/identity/spa-config');
    if (!response.ok) {
      throw new Error(`Failed to load SPA auth config (${response.status})`);
    }
    return await response.json();
  } catch (error) {
    console.error('Falling back to default SPA auth config.', error);
    return DEFAULT_SPA_AUTH_CONFIG;
  }
};

loadSpaConfig()
  .then((spaConfig) => bootstrapApplication(AppComponent, createAppConfig(spaConfig)))
  .catch((err) => console.error(err));
