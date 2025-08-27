import {
  Component,
  AfterViewInit,
  ViewChild,
  ChangeDetectionStrategy,
  computed,
  input,
  output,
} from '@angular/core';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatSortModule, MatSort, Sort } from '@angular/material/sort';

/**
 * Interface for defining a column header in the grid.
 */
export interface GridColumnHeader {
  /** The name of the property for which this header will be used. */
  value: string;
  /** The string to be rendered as the column header. */
  label: string;
}

/**
 * This component displays data in a sortable grid using Angular Material's MatTable.
 *
 * It encapsulates the `MatTableDataSource` logic internally, making it a "smarter"
 * presentational component. It uses modern Angular features like `input()` and
 * signals to manage its state and react to changes.
 *
 * Key features:
 * - Uses `ChangeDetectionStrategy.OnPush` for performance.
 * - Manages its own `MatTableDataSource` instance to leverage built-in sorting.
 * - Connects the `MatSort` directive via `@ViewChild` and `ngAfterViewInit`.
 * - The `dataSource` input uses a `set` accessor to handle incoming data updates.
 * - The `columns` is a computed signal, ensuring it's always in sync with `displayedColumns`.
 */
@Component({
  selector: 'lib-grid',
  standalone: true, // Marking the component as standalone
  imports: [MatTableModule, MatSortModule],
  changeDetection: ChangeDetectionStrategy.OnPush, // Use OnPush for better performance
  templateUrl: './grid.component.html',
  styleUrl: './grid.component.scss',
})
export class GridComponent<T> implements AfterViewInit {
  /**
   * The array of column headers to be displayed.
   */
  readonly displayedColumns = input<GridColumnHeader[]>([]);

  /**
   * The data array to be displayed in the grid.
   */
  readonly dataSource = input<T[]>([]);

  /**
   * A boolean to enable or disable sorting on the grid.
   */
  readonly sorting = input(false);

  /**
   * Emits a `Sort` event when a column header is clicked. This can be used
   * by the parent component to react to sorting changes, even if the table
   * handles the sort visually.
   */
  readonly sortChange = output<Sort>();

  /**
   * A private signal to hold the `MatTableDataSource` instance.
   * This signal is computed based on the `dataSource` input,
   * ensuring the `MatTableDataSource` is always up-to-date with the data.
   */
  protected dataSourceSignal = computed(() => {
    const dataSource = new MatTableDataSource(this.dataSource());
    if (this.sort) {
      dataSource.sort = this.sort;
    }
    return dataSource;
  });

  /**
   * ViewChild to get a reference to the `MatSort` directive.
   */
  @ViewChild(MatSort) sort!: MatSort;

  /**
   * A computed signal that derives the column names from the displayedColumns input.
   */
  protected columns = computed(() =>
    this.displayedColumns().map((column) => column.value)
  );

  /**
   * Lifecycle hook to connect the `MatSort` instance to the `MatTableDataSource`.
   * This is required for the built-in sorting logic to work.
   */
  ngAfterViewInit() {
    // We check for the data source and the sort to avoid errors
    if (this.dataSourceSignal() && this.sort) {
      this.dataSourceSignal().sort = this.sort;
      // Also subscribe to the sort events to re-emit for the parent
      this.sort.sortChange.subscribe((sortState) =>
        this.sortChange.emit(sortState)
      );
    }
  }
}
