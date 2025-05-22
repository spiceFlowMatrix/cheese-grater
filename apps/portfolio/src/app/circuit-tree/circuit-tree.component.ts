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
  x?: number; // Target X position
  y?: number; // Target Y position
  currentX?: number; // Current X position for animation
  currentY?: number; // Current Y position for animation
  currentOpacity?: number; // Current opacity for animation
  targetOpacity?: number; // Target opacity
  width?: number; // Node width for click detection
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
  private nodeSpacing = 20;
  private levelSpacing = 70;
  private nodes: CircuitNode[] = [];
  private animationStartTime: number | null = null;
  private animationDuration = 500; // Animation duration in ms

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
    this.initializePositions(this.circuitData, 400, 30);
  }

  ngAfterViewInit() {
    const canvas = this.canvasRef.nativeElement;
    this.ctx = canvas.getContext('2d')!;
    canvas.width = 800;
    canvas.height = 600;
    this.renderTree(true);
  }

  private initializePositions(node: CircuitNode, x: number, y: number) {
    node.currentX = x - this.nodeWidth / 2;
    node.currentY = y;
    node.x = node.currentX;
    node.y = y;
    node.currentOpacity = 1;
    node.targetOpacity = 1;
    if (node.expanded && node.children) {
      const childCount = node.children.length;
      const totalWidth = (childCount - 1) * this.nodeSpacing;
      const startX = x - totalWidth / 2;
      node.children.forEach((child, i) => {
        this.initializePositions(
          child,
          startX + i * this.nodeSpacing,
          y + this.levelSpacing
        );
        child.currentOpacity = 1;
        child.targetOpacity = 1;
      });
    } else if (node.children) {
      // Initialize collapsed children at parent's position with opacity 0
      node.children.forEach((child) => {
        this.initializePositions(child, x, y);
        child.currentOpacity = 0;
        child.targetOpacity = 0;
      });
    }
  }

  private calculateLayout(node: CircuitNode, level: number): number {
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

  private assignTargetPositions(
    node: CircuitNode,
    x: number,
    y: number,
    handleOpacity = true
  ) {
    node.x = x - this.nodeWidth / 2;
    node.y = y;
    if (handleOpacity) node.targetOpacity = node.expanded ? 1 : 0;
    if (node.expanded && node.children) {
      let startX = x - (node.subtreeWidth || this.nodeWidth) / 2;
      for (const child of node.children) {
        const childSubtreeWidth = child.subtreeWidth || this.nodeWidth;
        const childX = startX + childSubtreeWidth / 2;
        const childY = y + this.levelSpacing;
        child.targetOpacity = 1;
        this.assignTargetPositions(child, childX, childY, false);
        startX += childSubtreeWidth + this.nodeSpacing;
      }
    } else if (node.children) {
      // Collapsed children target parent's position with opacity 0
      node.children.forEach((child) => {
        child.x = node.x;
        child.y = node.y;
        child.targetOpacity = 0;
        this.assignTargetPositions(child, x, y);
      });
    }
  }

  private renderTree(animate: boolean) {
    this.nodes = [];
    this.calculateLayout(this.circuitData, 0);
    this.assignTargetPositions(this.circuitData, 400, 30);
    if (animate) {
      this.animationStartTime = performance.now();
      this.animate();
    } else {
      this.updateCurrentPositions();
      this.drawFrame();
    }
  }

  private updateCurrentPositions() {
    const traverse = (node: CircuitNode) => {
      node.currentX = node.x;
      node.currentY = node.y;
      node.currentOpacity = node.targetOpacity;
      if (node.children) {
        node.children.forEach(traverse);
      }
    };
    traverse(this.circuitData);
  }

  private animate() {
    if (!this.animationStartTime) return;
    const elapsed = performance.now() - this.animationStartTime;
    const progress = Math.min(elapsed / this.animationDuration, 1);

    // Linear interpolation for position and opacity
    const traverse = (node: CircuitNode) => {
      if (node.x !== undefined && node.currentX !== undefined) {
        node.currentX = node.currentX + (node.x - node.currentX) * progress;
      }
      if (node.y !== undefined && node.currentY !== undefined) {
        node.currentY = node.currentY + (node.y - node.currentY) * progress;
      }
      if (
        node.currentOpacity !== undefined &&
        node.targetOpacity !== undefined
      ) {
        node.currentOpacity =
          node.currentOpacity +
          (node.targetOpacity - node.currentOpacity) * progress;
      }
      if (node.children) {
        node.children.forEach(traverse);
      }
    };
    traverse(this.circuitData);

    this.drawFrame();

    if (progress < 1) {
      requestAnimationFrame(() => this.animate());
    } else {
      this.animationStartTime = null;
    }
  }

  private drawFrame() {
    this.nodes = [];
    this.ctx.clearRect(
      0,
      0,
      this.canvasRef.nativeElement.width,
      this.canvasRef.nativeElement.height
    );
    this.drawNode(this.circuitData);
  }

  private drawNode(node: CircuitNode) {
    // Skip drawing if opacity is effectively 0
    if (node.currentOpacity === 0) return;

    const x = node.currentX! + this.nodeWidth / 2; // Center of node
    const y = node.currentY!;
    const text = `${node.name} (${node.type})`;
    this.ctx.font = '14px Arial';
    const cornerRadius = 8;

    // Store node for click detection (only if visible)
    if (node.currentOpacity! > 0.1) {
      node.width = this.nodeWidth;
      node.height = this.nodeHeight;
      this.nodes.push(node);
    }

    // Save context to apply opacity
    this.ctx.save();
    this.ctx.globalAlpha = node.currentOpacity!;

    // Draw rounded square
    this.ctx.beginPath();
    this.ctx.roundRect(
      node.currentX!,
      y,
      this.nodeWidth,
      this.nodeHeight,
      cornerRadius
    );
    this.ctx.fillStyle = '#e6f3fa';
    this.ctx.fill();
    this.ctx.strokeStyle = '#333';
    this.ctx.stroke();

    // Draw text
    this.ctx.fillStyle = '#333';
    this.ctx.textAlign = 'center';
    this.ctx.textBaseline = 'middle';
    this.ctx.fillText(
      (node.children?.length ? (node.expanded ? '▼ ' : '▶ ') : '') + text,
      x,
      y + this.nodeHeight / 2
    );
    this.ctx.textAlign = 'start';

    // Draw children if expanded and parent is visible
    if (node.expanded && node.children) {
      for (const child of node.children) {
        const childX = child.currentX! + this.nodeWidth / 2;
        const childY = child.currentY!;

        // Draw connecting line with interpolated opacity
        this.ctx.save();
        this.ctx.globalAlpha = Math.min(
          node.currentOpacity!,
          child.currentOpacity!
        );
        this.ctx.beginPath();
        this.ctx.moveTo(x, y + this.nodeHeight);
        this.ctx.lineTo(childX, childY);
        this.ctx.strokeStyle = '#ccc';
        this.ctx.stroke();
        this.ctx.restore();

        // Draw child
        this.drawNode(child);
      }
    }

    this.ctx.restore();
  }

  onCanvasClick(event: MouseEvent) {
    const rect = this.canvasRef.nativeElement.getBoundingClientRect();
    const x = event.clientX - rect.left;
    const y = event.clientY - rect.top;

    // Find clicked node
    for (const node of this.nodes) {
      if (
        node.currentX &&
        node.currentY &&
        node.width &&
        node.height &&
        x >= node.currentX &&
        x <= node.currentX + node.width &&
        y >= node.currentY &&
        y <= node.currentY + node.height &&
        node.children?.length
      ) {
        node.expanded = !node.expanded;
        this.renderTree(true);
        break;
      }
    }
  }
}
