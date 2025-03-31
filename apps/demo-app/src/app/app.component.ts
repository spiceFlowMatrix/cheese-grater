import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { GridComponent} from '@vaenthecheesegrater/grid';
@Component({
  imports: [GridComponent, RouterModule],
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  title = 'demo-app';
}
