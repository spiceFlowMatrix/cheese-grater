import {
  AfterViewInit,
  Component,
  EventEmitter,
  Input,
  Output,
  ViewChild,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatSort } from '@angular/material/sort';

export interface GridColumnHeader {
  /** The name of the property for which this header will be used */
  value: string;
  /** This string will be rendered as the column header */
  label: string;
}

@Component({
  selector: 'lib-grid',
  imports: [CommonModule, MatTableModule, MatSortModule],
  templateUrl: './grid.component.html',
  styleUrl: './grid.component.css',
})
export class GridComponent<T> implements AfterViewInit {
  //#region Inputs
  @Input() displayedColumns: Array<GridColumnHeader> = [];
  @Input() get dataSource(): Array<T> {
    return this._dataSource ? this._dataSource.data : [];
  }
  set dataSource(value: Array<T>) {
    if (!this._dataSource) {
      this._dataSource = new MatTableDataSource(value);
      return;
    }
    this._dataSource.data = value;
  }
  @Input() sorting = false;
  //#endregion

  //#region Outputs
  @Output() sortChange = new EventEmitter<Sort>();
  //#endregion

  //#region Internal Properties
  protected _dataSource: MatTableDataSource<T> | null = null;
  @ViewChild(MatSort) sort: MatSort | null = null;
  //#endregion

  //#region Angular Lifecycle Hooks
  ngAfterViewInit() {
    if (this._dataSource) this._dataSource.sort = this.sort;
  }
  //#endregion

  protected get _columns(): string[] {
    return this.displayedColumns.map((column) => column.value);
  }

  handleSortChange(sortState: Sort) {
    this.sortChange.emit(sortState);
  }
}
