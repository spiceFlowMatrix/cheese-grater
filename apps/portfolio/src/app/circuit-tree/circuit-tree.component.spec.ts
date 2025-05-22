import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CircuitTreeComponent } from './circuit-tree.component';

describe('CircuitTreeComponent', () => {
  let component: CircuitTreeComponent;
  let fixture: ComponentFixture<CircuitTreeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CircuitTreeComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(CircuitTreeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
