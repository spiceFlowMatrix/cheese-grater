import {
  Component,
  ElementRef,
  ViewChild,
  AfterViewInit,
  OnInit,
} from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-circuit-tree',
  imports: [CommonModule],
  templateUrl: './circuit-tree.component.html',
  styleUrls: ['./circuit-tree.component.scss'],
})
export class CircuitTreeComponent implements AfterViewInit {
  @ViewChild('brainCanvas') brainCanvas!: ElementRef<HTMLCanvasElement>;
  private ctx!: CanvasRenderingContext2D;
  private animationProgress = 0;

  // Central microchip definition
  private microchip = { x: 300, y: 200, width: 60, height: 40, pinsPerSide: 3 };

  // Nodes array: approximate positions for brain silhouette and internal structure
  private nodes = [
    // Outer perimeter nodes (brain outline, clockwise from top-left)
    { x: 200, y: 100 }, { x: 250, y: 80 }, { x: 300, y: 70 }, { x: 350, y: 80 }, { x: 400, y: 100 }, // Top
    { x: 450, y: 150 }, { x: 460, y: 200 }, { x: 450, y: 250 }, { x: 400, y: 300 }, // Right
    { x: 350, y: 320 }, { x: 300, y: 330 }, { x: 250, y: 320 }, { x: 200, y: 300 }, // Bottom
    { x: 150, y: 250 }, { x: 140, y: 200 }, { x: 150, y: 150 }, // Left
    // Internal nodes (near microchip and along branches)
    { x: 270, y: 170 }, { x: 330, y: 170 }, { x: 270, y: 230 }, { x: 330, y: 230 }, // Near microchip
    { x: 300, y: 130 }, { x: 300, y: 270 }, { x: 220, y: 200 }, { x: 380, y: 200 } // Along branches
  ];

  // Polylines array: branching from microchip pins to nodes
  private polylines = [
    // Top pins (3 pins from top of microchip)
    [{ x: 290, y: 180 }, { x: 290, y: 130 }, { x: 250, y: 80 }, { x: 200, y: 100 }], // Left branch
    [{ x: 300, y: 180 }, { x: 300, y: 130 }, { x: 300, y: 70 }], // Center branch
    [{ x: 310, y: 180 }, { x: 310, y: 130 }, { x: 350, y: 80 }, { x: 400, y: 100 }], // Right branch
    // Right pins (3 pins from right of microchip)
    [{ x: 330, y: 190 }, { x: 380, y: 190 }, { x: 450, y: 150 }], // Top-right branch
    [{ x: 330, y: 200 }, { x: 380, y: 200 }, { x: 460, y: 200 }], // Middle-right branch
    [{ x: 330, y: 210 }, { x: 380, y: 210 }, { x: 450, y: 250 }, { x: 400, y: 300 }], // Bottom-right branch
    // Bottom pins (3 pins from bottom of microchip)
    [{ x: 290, y: 220 }, { x: 290, y: 270 }, { x: 250, y: 320 }, { x: 200, y: 300 }], // Left branch
    [{ x: 300, y: 220 }, { x: 300, y: 270 }, { x: 300, y: 330 }], // Center branch
    [{ x: 310, y: 220 }, { x: 310, y: 270 }, { x: 350, y: 320 }, { x: 400, y: 300 }], // Right branch
    // Left pins (3 pins from left of microchip)
    [{ x: 270, y: 190 }, { x: 220, y: 190 }, { x: 150, y: 150 }], // Top-left branch
    [{ x: 270, y: 200 }, { x: 220, y: 200 }, { x: 140, y: 200 }], // Middle-left branch
    [{ x: 270, y: 210 }, { x: 220, y: 210 }, { x: 150, y: 250 }, { x: 200, y: 300 }] // Bottom-left branch
  ];

  ngAfterViewInit(): void {
    this.ctx = this.brainCanvas.nativeElement.getContext('2d')!;
    this.animate();
  }

  private animate(): void {
    this.animationProgress += 0.005;
    if (this.animationProgress > 1) this.animationProgress = 0;
    this.draw();
    requestAnimationFrame(() => this.animate());
  }

  private draw(): void {
    this.ctx.clearRect(0, 0, 600, 400);

    // Draw microchip
    this.ctx.fillStyle = '#555';
    this.ctx.fillRect(this.microchip.x - this.microchip.width / 2, this.microchip.y - this.microchip.height / 2, this.microchip.width, this.microchip.height);

    // Draw polylines with electric animation
    const electricColor = '#00ffcc';
    const defaultColor = '#333';

    this.polylines.forEach(polyline => {
      const totalLength = this.calculateTotalLength(polyline);
      const currentLength = this.animationProgress * totalLength;
      const { filledPath, remainingPath } = this.getPartialPath(polyline, currentLength);

      // Draw filled path (electric color)
      this.ctx.beginPath();
      this.ctx.moveTo(filledPath[0].x, filledPath[0].y);
      for (let i = 1; i < filledPath.length; i++) {
        this.ctx.lineTo(filledPath[i].x, filledPath[i].y);
      }
      this.ctx.strokeStyle = electricColor;
      this.ctx.lineWidth = 2;
      this.ctx.stroke();

      // Draw remaining path (default color)
      if (remainingPath.length > 1) {
        this.ctx.beginPath();
        this.ctx.moveTo(remainingPath[0].x, remainingPath[0].y);
        for (let i = 1; i < remainingPath.length; i++) {
          this.ctx.lineTo(remainingPath[i].x, remainingPath[i].y);
        }
        this.ctx.strokeStyle = defaultColor;
        this.ctx.lineWidth = 2;
        this.ctx.stroke();
      }
    });

    // Draw nodes
    this.nodes.forEach(node => {
      this.ctx.beginPath();
      this.ctx.arc(node.x, node.y, 5, 0, 2 * Math.PI);
      this.ctx.fillStyle = '#fff';
      this.ctx.fill();
      this.ctx.strokeStyle = '#000';
      this.ctx.stroke();
    });
  }

  private calculateTotalLength(polyline: { x: number, y: number }[]): number {
    let length = 0;
    for (let i = 0; i < polyline.length - 1; i++) {
      const p1 = polyline[i];
      const p2 = polyline[i + 1];
      length += Math.sqrt((p2.x - p1.x) ** 2 + (p2.y - p1.y) ** 2);
    }
    return length;
  }

  private getPartialPath(polyline: { x: number, y: number }[], currentLength: number): { filledPath: { x: number, y: number }[], remainingPath: { x: number, y: number }[] } {
    let remainingLength = currentLength;
    const filledPath = [polyline[0]];

    for (let i = 0; i < polyline.length - 1; i++) {
      const p1 = polyline[i];
      const p2 = polyline[i + 1];
      const segmentLength = Math.sqrt((p2.x - p1.x) ** 2 + (p2.y - p1.y) ** 2);

      if (remainingLength >= segmentLength) {
        filledPath.push(p2);
        remainingLength -= segmentLength;
      } else {
        const t = remainingLength / segmentLength;
        const x = p1.x + t * (p2.x - p1.x);
        const y = p1.y + t * (p2.y - p1.y);
        filledPath.push({ x, y });
        const remainingPath = [{ x, y }, ...polyline.slice(i + 1)];
        return { filledPath, remainingPath };
      }
    }
    return { filledPath: polyline, remainingPath: [] };
  }
}
