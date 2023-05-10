import { TestBed } from '@angular/core/testing';

import { CompanyDepartmentsService } from './company-departments.service';

describe('CompanyDepartmentsService', () => {
  let service: CompanyDepartmentsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CompanyDepartmentsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
