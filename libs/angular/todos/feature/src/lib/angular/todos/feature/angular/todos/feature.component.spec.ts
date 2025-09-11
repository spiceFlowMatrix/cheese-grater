import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AngularTodosFeatureComponent } from './feature.component';

describe('AngularTodosFeatureComponent', () => {
  let component: AngularTodosFeatureComponent;
  let fixture: ComponentFixture<AngularTodosFeatureComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AngularTodosFeatureComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(AngularTodosFeatureComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
