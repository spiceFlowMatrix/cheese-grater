import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AuthRedirectPanelComponent } from './auth-redirect-panel.component';

describe('AuthRedirectPanelComponent', () => {
  let component: AuthRedirectPanelComponent;
  let fixture: ComponentFixture<AuthRedirectPanelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AuthRedirectPanelComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(AuthRedirectPanelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
