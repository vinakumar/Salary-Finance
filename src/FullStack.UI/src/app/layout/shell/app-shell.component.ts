import { Component, inject, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatSidenavModule, MatSidenav } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatSidenavModule,
    MatToolbarModule,
    MatListModule,
    MatIconModule,
    MatButtonModule,
  ],
  template: `
    <mat-sidenav-container class="shell-container">
      <mat-sidenav #sidenav [mode]="(isHandset$ | async) ? 'over' : 'side'" [opened]="(isHandset$ | async) === false">
        <mat-toolbar color="primary">
          <span>FullStack</span>
        </mat-toolbar>
        <mat-nav-list>
          <a mat-list-item routerLink="/products" routerLinkActive="active">
            <mat-icon matListItemIcon>inventory_2</mat-icon>
            <span matListItemTitle>Products</span>
          </a>
          <a mat-list-item routerLink="/taxonomy" routerLinkActive="active">
            <mat-icon matListItemIcon>account_tree</mat-icon>
            <span matListItemTitle>Taxonomy</span>
          </a>
        </mat-nav-list>
      </mat-sidenav>

      <mat-sidenav-content>
        <mat-toolbar>
          @if (isHandset$ | async) {
            <button mat-icon-button (click)="sidenav.toggle()" aria-label="Toggle sidebar">
              <mat-icon>menu</mat-icon>
            </button>
          }
          <span>FullStack Product Catalog</span>
          <span class="spacer"></span>
          <button mat-icon-button (click)="toggleTheme()" aria-label="Toggle dark mode">
            <mat-icon>{{ isDark ? 'light_mode' : 'dark_mode' }}</mat-icon>
          </button>
        </mat-toolbar>
        <main>
          <router-outlet></router-outlet>
        </main>
      </mat-sidenav-content>
    </mat-sidenav-container>
  `,
  styles: [
    `
      .shell-container {
        height: 100vh;
      }
      mat-sidenav {
        width: 260px;
        border-right: none;
        box-shadow: 2px 0 8px rgba(0, 0, 0, 0.06);
      }
      mat-sidenav mat-toolbar {
        font-size: 20px;
        font-weight: 700;
        letter-spacing: -0.5px;
      }
      .spacer {
        flex: 1 1 auto;
      }
      main {
        padding: 0;
        min-height: calc(100vh - 64px);
        background: #f5f7fa;
      }
      .active {
        background: rgba(0, 120, 212, 0.08) !important;
        border-right: 3px solid #0078d4;
      }
    `,
  ],
})
export class AppShellComponent {
  @ViewChild('sidenav') sidenav!: MatSidenav;
  private readonly breakpointObserver = inject(BreakpointObserver);

  isDark = false;

  isHandset$ = this.breakpointObserver.observe(Breakpoints.Handset).pipe(map((result) => result.matches));

  toggleTheme() {
    this.isDark = !this.isDark;
    document.documentElement.classList.toggle('dark-theme', this.isDark);
  }
}
