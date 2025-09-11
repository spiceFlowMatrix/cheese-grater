import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AngularSharedCoreComponent } from './core.component';

describe('AngularSharedCoreComponent', () => {
  let component: AngularSharedCoreComponent;
  let fixture: ComponentFixture<AngularSharedCoreComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AngularSharedCoreComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(AngularSharedCoreComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
