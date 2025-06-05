import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PartControlComponent } from './part-control.component';

describe('DepartmentEmployeesComponent', () => {
  let component: PartControlComponent;
  let fixture: ComponentFixture<PartControlComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PartControlComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PartControlComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
