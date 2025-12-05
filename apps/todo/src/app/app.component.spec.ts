import { TestBed } from '@angular/core/testing';
import { AppComponent } from './app.component';
import { RouterModule } from '@angular/router';
import Keycloak from 'keycloak-js';
import { SpaAuthConfigService } from './core/services/spa-auth-config.service';

// Mock Keycloak
const mockKeycloak = {
  init: jest.fn().mockResolvedValue(true),
  login: jest.fn(),
  logout: jest.fn(),
  token: 'mock-token',
};

const mockSpaAuthConfigService = {
  loadConfig: jest.fn().mockResolvedValue({
    authServerUrl: 'http://localhost:8081/',
    realm: 'Test',
    clientId: 'todo-web',
    redirectUri: 'http://localhost:4200/',
    logoutRedirectUri: 'http://localhost:4200/',
    requireHttps: false,
  }),
};

describe('AppComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppComponent, RouterModule.forRoot([])],
      providers: [
        { provide: Keycloak, useValue: mockKeycloak },
        { provide: SpaAuthConfigService, useValue: mockSpaAuthConfigService },
      ],
    }).compileComponents();
  });

  it('should render title', () => {
    const fixture = TestBed.createComponent(AppComponent);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('app-header')).toBeTruthy();
  });

  it(`should have as title 'todo'`, () => {
    const fixture = TestBed.createComponent(AppComponent);
    const app = fixture.componentInstance;
    expect(app.title).toEqual('todo');
  });
});
