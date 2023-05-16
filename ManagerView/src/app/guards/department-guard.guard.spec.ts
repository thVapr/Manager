import { TestBed } from '@angular/core/testing';

import { DepartmentGuard } from './department-guard.guard';

describe('DepartmentGuard', () => {
  let guard: DepartmentGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(DepartmentGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
