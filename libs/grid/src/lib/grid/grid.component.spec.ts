import { ComponentFixture, TestBed } from '@angular/core/testing';
import { GridColumnHeader, GridComponent } from './grid.component';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { TestbedHarnessEnvironment } from '@angular/cdk/testing/testbed';
import { MatTableHarness } from '@angular/material/table/testing';
import { HarnessLoader } from '@angular/cdk/testing';
import { Component } from '@angular/core';
/**
 * A simple interface for the mock data.
 */
interface Person {
  name: string;
  age: number;
}

/**
 * A mock parent component to host the grid and provide data.
 * This is a more robust way to test components that use inputs and outputs.
 */
@Component({
  template: `
    <lib-grid
      [displayedColumns]="displayedColumns"
      [dataSource]="dataSource"
      [sorting]="sorting"
      (sortChange)="handleSortChange($event)"
    >
    </lib-grid>
  `,
  standalone: true,
  imports: [GridComponent, MatTableModule, MatSortModule],
})
export class TestHostComponent {
  displayedColumns: GridColumnHeader[] = [];
  dataSource: Person[] = [];
  sorting = false;
  sortChangeOutput: Sort | null = null;

  handleSortChange(sortState: Sort) {
    this.sortChangeOutput = sortState;
  }
}

describe('GridComponent', () => {
  const mockColumns: GridColumnHeader[] = [
    { value: 'name', label: 'Name' },
    { value: 'age', label: 'Age' },
  ];
  const mockDataSource: Person[] = [
    { name: 'John', age: 30 },
    { name: 'Jane', age: 25 },
    { name: 'Alex', age: 40 },
  ];

  let hostFixture: ComponentFixture<TestHostComponent>;
  let hostComponent: TestHostComponent;
  let gridComponent: GridComponent<Person>;
  let loader: HarnessLoader;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        MatTableModule,
        MatSortModule,
        NoopAnimationsModule,
        GridComponent,
        TestHostComponent,
      ],
      providers: [],
    }).compileComponents();

    hostFixture = TestBed.createComponent(TestHostComponent);
    hostComponent = hostFixture.componentInstance;
    loader = TestbedHarnessEnvironment.loader(hostFixture);

    // Set up the inputs on the host component
    hostComponent.displayedColumns = mockColumns;
    hostComponent.dataSource = mockDataSource;
    hostComponent.sorting = true;

    hostFixture.detectChanges();
    gridComponent = hostFixture.debugElement.children[0].componentInstance;
  });

  it('should create', () => {
    expect(gridComponent).toBeTruthy();
  });

  it('should render correct columns and data using harnesses', async () => {
    const table = await loader.getHarness(MatTableHarness);

    const headers = await table.getHeaderRows();
    const headerText = await headers[0].getCellTextByIndex();
    expect(headerText).toEqual(['Name', 'Age']);

    const rows = await table.getRows();
    expect(rows.length).toBe(3);

    const firstRowCells = await rows[0].getCellTextByIndex();
    expect(firstRowCells).toEqual(['John', '30']);

    const secondRowCells = await rows[1].getCellTextByIndex();
    expect(secondRowCells).toEqual(['Jane', '25']);
  });

  it('should emit a sortChange event when a header is clicked', async () => {
    // Spy on the public output property
    const sortSpy = jest.spyOn(hostComponent, 'handleSortChange');

    const table = await loader.getHarness(MatTableHarness);
    const headers = await table.getHeaderRows();
    const nameHeaderCell = (
      await headers[0].getCells({ columnName: 'name' })
    )[0];

    // Click the header to trigger the sort event
    await (await nameHeaderCell.host()).click();

    const ascSort = {
      active: 'name',
      direction: 'asc',
    };

    // Verify the host component's output property was updated
    expect(hostComponent.sortChangeOutput).toEqual(ascSort);
    expect(sortSpy).toHaveBeenLastCalledWith(ascSort);

    // Click again to change direction
    await (await nameHeaderCell.host()).click();

    const descSort = {
      active: 'name',
      direction: 'desc',
    };

    expect(hostComponent.sortChangeOutput).toEqual(descSort);
    expect(sortSpy).toHaveBeenLastCalledWith(descSort);
    expect(sortSpy).toHaveBeenCalledTimes(2);
  });

  it('should sort the data when a header is clicked', async () => {
    // The component's internal MatTableDataSource handles this, so we just check the result.
    const table = await loader.getHarness(MatTableHarness);
    const headers = await table.getHeaderRows();
    const ageHeaderCell = (await headers[0].getCells({ columnName: 'age' }))[0];

    // Click the age header to sort by age ascending
    await (await ageHeaderCell.host()).click();

    // Get the sorted rows and assert the order
    let rows = await table.getRows();
    let firstRowCells = await rows[0].getCellTextByIndex();
    let secondRowCells = await rows[1].getCellTextByIndex();
    let thirdRowCells = await rows[2].getCellTextByIndex();

    expect(firstRowCells).toEqual(['Jane', '25']);
    expect(secondRowCells).toEqual(['John', '30']);
    expect(thirdRowCells).toEqual(['Alex', '40']);

    // Click again to sort descending
    await (await ageHeaderCell.host()).click();

    rows = await table.getRows();
    firstRowCells = await rows[0].getCellTextByIndex();
    secondRowCells = await rows[1].getCellTextByIndex();
    thirdRowCells = await rows[2].getCellTextByIndex();

    expect(firstRowCells).toEqual(['Alex', '40']);
    expect(secondRowCells).toEqual(['John', '30']);
    expect(thirdRowCells).toEqual(['Jane', '25']);
  });
});
