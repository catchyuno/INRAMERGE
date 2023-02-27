import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddMBCComponent } from './add-mbc.component';

describe('AddMBCComponent', () => {
  let component: AddMBCComponent;
  let fixture: ComponentFixture<AddMBCComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddMBCComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddMBCComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
