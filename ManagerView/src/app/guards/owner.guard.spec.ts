import { TestBed } from '@angular/core/testing';
import { CanActivateFn } from '@angular/router';

import { OwnerGuard } from './owner.guard';

describe('ownerGuard', () => {
  let guard: OwnerGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(OwnerGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
