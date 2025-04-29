import { TestBed } from '@angular/core/testing';

import { PartGuard } from './part-guard.guard';

describe('DepartmentGuard', () => {
  let guard: PartGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(PartGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
