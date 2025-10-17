import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HeaderComponent } from './header.component';
import { faker } from '@faker-js/faker';
import { Component } from '@angular/core';
import { HarnessLoader } from '@angular/cdk/testing';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatToolbarHarness } from '@angular/material/toolbar/testing';

@Component({
  template: `
    <lib-header [title]="title" [subtitle]="subtitle"> </lib-header>
  `,
  standalone: true,
  imports: [HeaderComponent],
})
export class TestHostComponent {
  title = '';
  subtitle = '';
}

describe('HeaderComponent', () => {
  let component: HeaderComponent;
  let hostFixture: ComponentFixture<TestHostComponent>;
  let hostComponent: TestHostComponent;
  let loader: HarnessLoader;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HeaderComponent, TestHostComponent],
    }).compileComponents();

    hostFixture = TestBed.createComponent(TestHostComponent);
    hostComponent = hostFixture.componentInstance;

    loader = TestbedHarnessEnvironment.loader(hostFixture);

    hostFixture.detectChanges();

    component = hostFixture.debugElement.children[0].componentInstance;
  });

  it('should create using MatToolbar', async () => {
    expect(component).toBeTruthy();

    const toolbar = await loader.getHarness(MatToolbarHarness);

    expect(toolbar).toBeTruthy();
  });

  it('given `title` input, renders header text using `title` value', async () => {
    hostComponent.title = faker.word.noun();

    const toolbar = await loader.getHarness(MatToolbarHarness);
    const toolbarNative = TestbedHarnessEnvironment.getNativeElement(
      await toolbar.host()
    );

    expect(toolbarNative.textContent).toContain(hostComponent.title);
  });
});
