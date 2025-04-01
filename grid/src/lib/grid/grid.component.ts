import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';

export interface GridColumnHeader {
  /** The name of the property for which this header will be used */
  value: string;
  /** This string will be rendered as the column header */
  label: string;
}

@Component({
  selector: 'lib-grid',
  imports: [CommonModule, MatTableModule],
  templateUrl: './grid.component.html',
  styleUrl: './grid.component.css',
})
export class GridComponent<T> {
  @Input() displayedColumns: Array<GridColumnHeader> = [];
  @Input() dataSource: Array<T> = [];

  protected get _columns(): string[] {
    return this.displayedColumns.map((column) => column.value);
  }
}
