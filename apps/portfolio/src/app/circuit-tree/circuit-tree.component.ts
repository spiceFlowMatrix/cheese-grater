import {
  Component,
  ElementRef,
  ViewChild,
  AfterViewInit,
  OnInit,
} from '@angular/core';
import { CommonModule } from '@angular/common';

interface CircuitNode {
  name: string;
  type: string;
  children?: CircuitNode[];
  expanded?: boolean;
  x?: number; // Canvas X position
  y?: number; // Canvas Y position
  width?: number; // Node width for click detection and layout
  height?: number; // Node height for click detection
  subtreeWidth?: number; // Width of the node's subtree
}

@Component({
  selector: 'app-circuit-tree',
  imports: [CommonModule],
  templateUrl: './circuit-tree.component.html',
  styleUrls: ['./circuit-tree.component.scss'],
})
export class CircuitTreeComponent implements OnInit, AfterViewInit {
  @ViewChild('treeCanvas', { static: true })
  canvasRef!: ElementRef<HTMLCanvasElement>;
  private ctx!: CanvasRenderingContext2D;
  private nodeHeight = 40;
  private nodeWidth = 120;
  private nodeSpacing = 20; // Horizontal spacing between nodes
  private levelSpacing = 70; // Vertical spacing between levels
  private nodes: CircuitNode[] = [];

  circuitData: CircuitNode = {
    name: 'Main Circuit',
    type: 'node',
    expanded: true,
    children: [
      {
        name: 'R1',
        type: 'resistor',
        children: [
          { name: 'C1', type: 'capacitor' },
          { name: 'L1', type: 'inductor' },
        ],
      },
      {
        name: 'R2',
        type: 'resistor',
        children: [
          { name: 'C2', type: 'capacitor' },
          {
            name: 'Node1',
            type: 'node',
            children: [{ name: 'R3', type: 'resistor' }],
          },
        ],
      },
    ],
  };

  ngOnInit() {
    this.circuitData.expanded = true;
  }

  ngAfterViewInit() {
    const canvas = this.canvasRef.nativeElement;
    this.ctx = canvas.getContext('2d')!;
    canvas.width = 800;
    canvas.height = 600;
    this.renderTree();
  }

  private renderTree() {
    this.nodes = [];
    this.ctx.clearRect(
      0,
      0,
      this.canvasRef.nativeElement.width,
      this.canvasRef.nativeElement.height
    );
    this.calculateLayout(this.circuitData, 0);
    this.drawNode(this.circuitData, 400, 30);
  }

  private calculateLayout(node: CircuitNode, level: number): number {
    // Calculate subtree width
    let subtreeWidth = this.nodeWidth;
    if (node.expanded && node.children) {
      subtreeWidth = 0;
      for (const child of node.children) {
        subtreeWidth +=
          this.calculateLayout(child, level + 1) + this.nodeSpacing;
      }
      subtreeWidth = Math.max(subtreeWidth - this.nodeSpacing, this.nodeWidth);
    }
    node.subtreeWidth = subtreeWidth;
    return subtreeWidth;
  }

  private drawNode(node: CircuitNode, x: number, y: number) {
    const text = `${node.name} (${node.type})`;
    this.ctx.font = '14px Arial';
    const cornerRadius = 8;

    // Store node position for click detection
    node.x = x - this.nodeWidth / 2;
    node.y = y;
    node.width = this.nodeWidth;
    node.height = this.nodeHeight;
    this.nodes.push(node);

    // Draw rounded square
    this.ctx.beginPath();
    this.ctx.roundRect(
      node.x,
      node.y,
      this.nodeWidth,
      this.nodeHeight,
      cornerRadius
    );
    this.ctx.fillStyle = '#e6f3fa';
    this.ctx.fill();
    this.ctx.strokeStyle = '#333';
    this.ctx.stroke();

    // Draw text centered in the square
    this.ctx.fillStyle = '#333';
    this.ctx.textAlign = 'center';
    this.ctx.textBaseline = 'middle';
    this.ctx.fillText(
      (node.children?.length ? (node.expanded ? '▼ ' : '▶ ') : '') + text,
      x,
      y + this.nodeHeight / 2
    );
    this.ctx.textAlign = 'start';

    // Draw children if expanded
    if (node.expanded && node.children) {
      let startX = x - (node.subtreeWidth || this.nodeWidth) / 2;
      for (const child of node.children) {
        const childSubtreeWidth = child.subtreeWidth || this.nodeWidth;
        const childX = startX + childSubtreeWidth / 2;
        const childY = y + this.levelSpacing;

        // Draw connecting line
        this.ctx.beginPath();
        this.ctx.moveTo(x, y + this.nodeHeight);
        this.ctx.lineTo(childX, childY);
        this.ctx.strokeStyle = '#ccc';
        this.ctx.stroke();

        // Draw child
        this.drawNode(child, childX, childY);

        // Move to next child position
        startX += childSubtreeWidth + this.nodeSpacing;
      }
    }
  }

  onCanvasClick(event: MouseEvent) {
    const rect = this.canvasRef.nativeElement.getBoundingClientRect();
    const x = event.clientX - rect.left;
    const y = event.clientY - rect.top;

    // Find clicked node
    for (const node of this.nodes) {
      if (
        node.x &&
        node.y &&
        node.width &&
        node.height &&
        x >= node.x &&
        x <= node.x + node.width &&
        y >= node.y &&
        y <= node.y + node.height &&
        node.children?.length
      ) {
        node.expanded = !node.expanded;
        this.renderTree();
        break;
      }
    }
  }
}
