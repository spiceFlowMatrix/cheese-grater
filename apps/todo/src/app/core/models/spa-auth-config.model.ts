export interface SpaAuthConfig {
  authServerUrl: string;
  realm: string;
  clientId: string;
  redirectUri: string;
  logoutRedirectUri: string;
  requireHttps: boolean;
}
