import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AuthRedirectPanelComponent } from './auth-redirect-panel.component';
import { Component } from '@angular/core';
import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatButtonHarness } from '@angular/material/button/testing';
import { MatCardHarness, MatCardSection } from '@angular/material/card/testing';
import { faker } from '@faker-js/faker';

@Component({
  template: `
    <lib-auth-redirect-panel
      [showLogin]="showLogin"
      [showSignup]="showSignup"
      [loginButtonLabel]="loginButtonLabel"
      [signupButtonLabel]="signupButtonLabel"
      [titleLabel]="titleLabel"
      (loginClicked)="handleLoginClicked()"
      (signupClicked)="handleSignupClicked()"
    >
    </lib-auth-redirect-panel>
  `,
  standalone: true,
  imports: [AuthRedirectPanelComponent],
})
export class TestHostComponent {
  showLogin = true;
  showSignup = true;
  loginButtonLabel = '';
  signupButtonLabel = '';
  titleLabel = '';

  handleLoginClicked() {
    console.log('Login clicked');
  }
  handleSignupClicked() {
    console.log('Signup clicked');
  }
}

describe('AuthRedirectPanelComponent', () => {
  let hostFixture: ComponentFixture<TestHostComponent>;
  let hostComponent: TestHostComponent;
  let component: AuthRedirectPanelComponent;
  let loader: HarnessLoader;
  let loginButtonLabel: string;
  let signupButtonLabel: string;
  let titleLabel: string;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AuthRedirectPanelComponent, TestHostComponent],
      providers: [],
    }).compileComponents();

    loginButtonLabel = faker.word.verb();
    signupButtonLabel = faker.word.verb();
    titleLabel = faker.word.words(5);

    hostFixture = TestBed.createComponent(TestHostComponent);
    hostComponent = hostFixture.componentInstance;

    loader = TestbedHarnessEnvironment.loader(hostFixture);

    hostFixture.detectChanges();
    component = hostFixture.debugElement.children[0].componentInstance;
  });

  it('should create', () => {
    expect(hostComponent).toBeTruthy();
    expect(component).toBeTruthy();
  });

  it('should hold buttons in a card', async () => {
    hostComponent.showLogin = true;
    hostComponent.showSignup = true;
    hostComponent.loginButtonLabel = loginButtonLabel;
    hostComponent.signupButtonLabel = signupButtonLabel;
    hostComponent.titleLabel = titleLabel;

    const card = await loader.getHarness(MatCardHarness);

    expect(card).toBeTruthy();
    const titleText = await card.getSubtitleText();
    expect(titleText).toBe(titleLabel);

    const buttons = await (
      await card.getChildLoader(MatCardSection.CONTENT)
    ).getAllHarnesses(MatButtonHarness);
    const loginButtonText = await buttons[0].getText();
    expect(loginButtonText).toBe(loginButtonLabel);
    const signupButtonText = await buttons[1].getText();
    expect(signupButtonText).toBe(signupButtonLabel);
  });

  it('should display login button based on input', async () => {
    hostComponent.showLogin = true;
    hostComponent.loginButtonLabel = loginButtonLabel;
    const displayedLoginButton = await loader.getHarness(
      MatButtonHarness.with({ text: loginButtonLabel })
    );

    expect(displayedLoginButton).toBeTruthy();

    hostComponent.showLogin = false;
    hostFixture.detectChanges();

    expect(
      loader.getHarness(MatButtonHarness.with({ text: loginButtonLabel }))
    ).rejects.toThrow();
  });

  it('given default parameters, when Login button is clicked `loginClicked` event is triggered', async () => {
    hostComponent.showLogin = true;
    hostComponent.loginButtonLabel = loginButtonLabel;
    const clickSpy = jest.spyOn(hostComponent, 'handleLoginClicked');

    const button = await loader.getHarness(
      MatButtonHarness.with({ text: loginButtonLabel })
    );
    await button.click();

    expect(clickSpy).toHaveBeenCalled();
  });

  it('should display signup button based on input', async () => {
    hostComponent.showSignup = true;
    hostComponent.signupButtonLabel = signupButtonLabel;

    const displayedSignupButton = await loader.getHarness(
      MatButtonHarness.with({ text: signupButtonLabel })
    );

    expect(displayedSignupButton).toBeTruthy();

    hostComponent.showSignup = false;
    hostFixture.detectChanges();

    await expect(
      loader.getHarness(MatButtonHarness.with({ text: signupButtonLabel }))
    ).rejects.toThrow();
  });

  it('given default parameters, when Signup button is clicked `signupClicked` event is triggered', async () => {
    hostComponent.showLogin = true;
    hostComponent.signupButtonLabel = signupButtonLabel;
    const clickSpy = jest.spyOn(hostComponent, 'handleSignupClicked');

    const button = await loader.getHarness(
      MatButtonHarness.with({ text: signupButtonLabel })
    );
    await button.click();

    expect(clickSpy).toHaveBeenCalled();
  });
});
