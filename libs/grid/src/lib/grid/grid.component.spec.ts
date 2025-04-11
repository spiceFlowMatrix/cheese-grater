import { ComponentFixture, TestBed } from '@angular/core/testing';
import { GridColumnHeader, GridComponent } from './grid.component';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

interface Person {
  name: string;
  age: number;
}

describe('GridComponent', () => {
  const mockColumns: GridColumnHeader[] = [
    { value: 'name', label: 'Name' },
    { value: 'age', label: 'Age' },
  ];
  const mockDataSource: Person[] = [
    { name: 'John', age: 30 },
    { name: 'Jane', age: 25 },
  ];
  let component: GridComponent<Person>;
  let fixture: ComponentFixture<GridComponent<Person>>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        MatTableModule,
        MatSortModule,
        NoopAnimationsModule,
        GridComponent,
      ],
      providers: [],
    }).compileComponents();

    fixture = TestBed.createComponent(GridComponent<Person>);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should map displayedColumns to _columns', () => {
    const columns = mockColumns;
    component.displayedColumns = columns;
    expect(component['_columns']).toEqual(['name', 'age']);
  });

  it('should set dataSource correctly', () => {
    const data = mockDataSource;
    component.dataSource = data;
    expect(component['_dataSource']?.data).toEqual(data);
  });

  it('should render table with correct columns and data', () => {
    const columns = mockColumns;
    const data = mockDataSource;
    component.displayedColumns = columns;
    component.dataSource = data;
    fixture.detectChanges();

    const tableElement = fixture.nativeElement.querySelector('table');
    expect(tableElement).toBeTruthy();

    // Check headers
    const headerCells = tableElement.querySelectorAll('thead th');
    expect(headerCells.length).toBe(2);
    expect(headerCells[0].textContent).toContain('Name');
    expect(headerCells[1].textContent).toContain('Age');

    // Check rows
    const bodyRows = tableElement.querySelectorAll('tbody tr');
    expect(bodyRows.length).toBe(2);
    const firstRowCells = bodyRows[0].querySelectorAll('td');
    expect(firstRowCells[0].textContent).toContain('John');
    expect(firstRowCells[1].textContent).toContain('30');
  });

  it('should emit sortChange when MatSort changes', () => {
    const columns = mockColumns;
    const data = mockDataSource;
    component.displayedColumns = columns;
    component.dataSource = data;
    fixture.detectChanges();

    // Spy on the sortChange EventEmitter's emit method
    const sortSpy = jest.spyOn(component.sortChange, 'emit');
    const sortState: Sort = { active: 'name', direction: 'asc' };
    component.sort?.sortChange.emit(sortState);

    expect(sortSpy).toHaveBeenCalledWith(sortState);
  });

  it('should render sorted data when MatSort changes', () => {
    const columns = mockColumns;
    const data = mockDataSource;
    component.displayedColumns = columns;
    component.dataSource = data;
    fixture.detectChanges();

    component.sort?.sort({ id: 'age', start: 'desc', disableClear: false });
    fixture.detectChanges();

    const tableElement = fixture.nativeElement.querySelector('table');
    const bodyRows = tableElement.querySelectorAll('tbody tr');
    const firstRowCells = bodyRows[0].querySelectorAll('td');
    expect(firstRowCells[0].textContent).toContain('Jane');
    expect(firstRowCells[1].textContent).toContain('25');

    const secondRowCells = bodyRows[1].querySelectorAll('td');
    expect(secondRowCells[0].textContent).toContain('John');
    expect(secondRowCells[1].textContent).toContain('30');
  });
});
