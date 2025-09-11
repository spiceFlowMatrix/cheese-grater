import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AngularTodosCoreComponent } from './core.component';

describe('AngularTodosCoreComponent', () => {
  let component: AngularTodosCoreComponent;
  let fixture: ComponentFixture<AngularTodosCoreComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AngularTodosCoreComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(AngularTodosCoreComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
