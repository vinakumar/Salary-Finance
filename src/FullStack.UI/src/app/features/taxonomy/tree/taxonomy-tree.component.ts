import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTreeModule, MatTreeNestedDataSource } from '@angular/material/tree';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { NestedTreeControl } from '@angular/cdk/tree';
import { TaxonomyStore } from '../store/taxonomy.store';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { ProductTaxonomyNode } from '../../../api/generated/models';

@Component({
  selector: 'app-taxonomy-tree',
  standalone: true,
  imports: [CommonModule, MatTreeModule, MatIconModule, MatButtonModule, MatCardModule, LoadingSpinnerComponent],
  template: `
    <div class="page-container">
      <h1>Product Taxonomy</h1>

      @if (taxonomyStore.loading()) {
        <app-loading-spinner />
      } @else {
        <div class="taxonomy-layout">
          <mat-card class="tree-panel">
            <mat-card-header>
              <mat-card-title>Categories</mat-card-title>
            </mat-card-header>
            <mat-card-content>
              <mat-tree [dataSource]="dataSource" [treeControl]="treeControl">
                <mat-nested-tree-node *matTreeNodeDef="let node">
                  <div
                    class="mat-tree-node"
                    [class.selected]="selectedNode?.nodeId === node.nodeId"
                    (click)="selectNode(node)"
                  >
                    <button mat-icon-button disabled></button>
                    <mat-icon>{{ node.isLeaf ? 'description' : 'folder' }}</mat-icon>
                    <span>{{ node.label }}</span>
                  </div>
                </mat-nested-tree-node>
                <mat-nested-tree-node *matTreeNodeDef="let node; when: hasChild">
                  <div
                    class="mat-tree-node"
                    [class.selected]="selectedNode?.nodeId === node.nodeId"
                    (click)="selectNode(node)"
                  >
                    <button mat-icon-button matTreeNodeToggle>
                      <mat-icon>{{ treeControl.isExpanded(node) ? 'expand_more' : 'chevron_right' }}</mat-icon>
                    </button>
                    <mat-icon>{{ treeControl.isExpanded(node) ? 'folder_open' : 'folder' }}</mat-icon>
                    <span>{{ node.label }}</span>
                  </div>
                  <div [class.hidden]="!treeControl.isExpanded(node)">
                    <ng-container matTreeNodeOutlet></ng-container>
                  </div>
                </mat-nested-tree-node>
              </mat-tree>
            </mat-card-content>
          </mat-card>

          @if (selectedNode) {
            <mat-card class="detail-panel">
              <mat-card-header>
                <mat-card-title>{{ selectedNode.label }}</mat-card-title>
                <mat-card-subtitle>Node ID: {{ selectedNode.nodeId }}</mat-card-subtitle>
              </mat-card-header>
              <mat-card-content>
                <div class="detail-grid">
                  <div><strong>Depth:</strong> {{ selectedNode.depth }}</div>
                  <div><strong>Is Leaf:</strong> {{ selectedNode.isLeaf ? 'Yes' : 'No' }}</div>
                  <div><strong>Children:</strong> {{ selectedNode.children?.length || 0 }}</div>
                </div>
                @if (taxonomyStore.selectedPayload()) {
                  <h4>Payload Metadata</h4>
                  <pre>{{ taxonomyStore.selectedPayload() | json }}</pre>
                }
              </mat-card-content>
            </mat-card>
          }
        </div>
      }
    </div>
  `,
  styles: [
    `
      .taxonomy-layout {
        display: grid;
        grid-template-columns: 1fr 1fr;
        gap: 24px;
      }
      .tree-panel,
      .detail-panel {
        min-height: 400px;
      }
      .mat-tree-node {
        display: flex;
        align-items: center;
        gap: 4px;
        padding: 4px 0;
        cursor: pointer;
      }
      .mat-tree-node:hover {
        background: rgba(0, 0, 0, 0.04);
      }
      .mat-tree-node.selected {
        background: rgba(0, 0, 0, 0.08);
      }
      .hidden {
        display: none;
      }
      .detail-grid {
        display: grid;
        gap: 8px;
        padding: 16px 0;
      }
      pre {
        background: #f5f5f5;
        padding: 12px;
        border-radius: 4px;
        font-size: 12px;
        overflow-x: auto;
      }
      @media (max-width: 768px) {
        .taxonomy-layout {
          grid-template-columns: 1fr;
        }
      }
    `,
  ],
})
export class TaxonomyTreeComponent implements OnInit {
  readonly taxonomyStore = inject(TaxonomyStore);

  treeControl = new NestedTreeControl<ProductTaxonomyNode>((node) => node.children);
  dataSource = new MatTreeNestedDataSource<ProductTaxonomyNode>();
  selectedNode: ProductTaxonomyNode | null = null;

  hasChild = (_: number, node: ProductTaxonomyNode) => !node.isLeaf && !!node.children?.length;

  ngOnInit() {
    this.taxonomyStore.loadNodes({});
    // Subscribe to store changes
    setTimeout(() => {
      this.dataSource.data = this.taxonomyStore.nodes();
    }, 1000);
  }

  selectNode(node: ProductTaxonomyNode) {
    this.selectedNode = node;
    this.taxonomyStore.loadNode(node.nodeId);
  }
}
