import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FoinraManagementComponent } from './user-management.component';

describe('UserManagementComponent', () => {
  let component: FoinraManagementComponent;
  let fixture: ComponentFixture<FoinraManagementComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FoinraManagementComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FoinraManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
