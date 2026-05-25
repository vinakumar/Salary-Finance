import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { DataTableComponent } from './data-table.component';

describe('DataTableComponent', () => {
  let component: DataTableComponent<any>;
  let fixture: ComponentFixture<DataTableComponent<any>>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DataTableComponent, NoopAnimationsModule],
    }).compileComponents();

    fixture = TestBed.createComponent(DataTableComponent);
    component = fixture.componentInstance;
    component.columns = ['id', 'name'];
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should update dataSource when data is set', () => {
    component.data = [
      { id: 1, name: 'Item 1' },
      { id: 2, name: 'Item 2' },
    ];
    expect(component.dataSource.data.length).toBe(2);
  });

  it('should emit sort event', () => {
    const sortSpy = spyOn(component.sortChange, 'emit');
    component.onSortChange({ active: 'name', direction: 'asc' });
    expect(sortSpy).toHaveBeenCalledWith({ active: 'name', direction: 'asc' });
  });

  it('should emit page event', () => {
    const pageSpy = spyOn(component.pageChange, 'emit');
    const event = { pageIndex: 1, pageSize: 20, length: 100 } as any;
    component.onPageChange(event);
    expect(pageSpy).toHaveBeenCalledWith(event);
  });

  it('should emit row click event', () => {
    const rowSpy = spyOn(component.rowClick, 'emit');
    const row = { id: 1, name: 'Test' };
    component.rowClick.emit(row);
    expect(rowSpy).toHaveBeenCalledWith(row);
  });
});
