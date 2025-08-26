import { ComponentFixture, TestBed } from '@angular/core/testing';
import { GridColumnHeader, GridComponent } from './grid.component';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatTableHarness } from '@angular/material/table/testing';
import { HarnessLoader } from '@angular/cdk/testing';

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
  let loader: HarnessLoader;

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
    loader = TestbedHarnessEnvironment.loader(fixture);
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

  it('should click the header cell and trigger sorting', async () => {
    const columns = mockColumns;
    const data = mockDataSource;
    component.displayedColumns = columns;
    component.dataSource = data;
    component.sorting = true;

    // Get the MatTableHarness
    const table = await loader.getHarness(MatTableHarness);

    // Get the header rows
    const headerRows = await table.getHeaderRows();
    const ageHeader = (await headerRows[0].getCells({ columnName: 'age' }))[0];

    // Simulate clicking the header cell using host()
    await (await ageHeader.host()).click();
    await (await ageHeader.host()).click();

    expect(component.sort).toBeDefined();

    // if (component.sort)
    //   // Verify the outcome (e.g., sorting was applied)
    //   expect(component.sort.direction).toBe('asc'); // Adjust based on your sort logic

    // Get all the rows and make assertions on the displayed data
    const rows = await table.getRows();

    // Get the cells for the first row and assert the content
    const firstRowCells = await rows[0].getCells();
    const firstRowText = await Promise.all(
      firstRowCells.map((cell) => cell.getText())
    );
    expect(firstRowText).toEqual(['Jane', '25']);

    // Get the cells for the second row and assert the content
    const secondRowCells = await rows[0].getCells();
    const secondRowText = await Promise.all(
      secondRowCells.map((cell) => cell.getText())
    );
    expect(secondRowText).toEqual(['John', '30']);
  });
});
