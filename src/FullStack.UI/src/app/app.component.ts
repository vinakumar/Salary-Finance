import { Component } from '@angular/core';
import { AppShellComponent } from './layout/shell/app-shell.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [AppShellComponent],
  template: `<app-shell />`,
  styles: [],
})
export class AppComponent {
  title = 'FullStack.UI';
}
