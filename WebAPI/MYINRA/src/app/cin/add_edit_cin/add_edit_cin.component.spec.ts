/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Add_edit_cinComponent } from './add_edit_cin.component';

describe('Add_edit_cinComponent', () => {
  let component: Add_edit_cinComponent;
  let fixture: ComponentFixture<Add_edit_cinComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
       declarations: [ Add_edit_cinComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Add_edit_cinComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
