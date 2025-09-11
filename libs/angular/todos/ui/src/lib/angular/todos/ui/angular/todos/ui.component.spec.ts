import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AngularTodosUiComponent } from './ui.component';

describe('AngularTodosUiComponent', () => {
  let component: AngularTodosUiComponent;
  let fixture: ComponentFixture<AngularTodosUiComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AngularTodosUiComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(AngularTodosUiComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
