/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Add_edit_banqueComponent } from './add_edit_banque.component';

describe('Add_edit_banqueComponent', () => {
  let component: Add_edit_banqueComponent;
  let fixture: ComponentFixture<Add_edit_banqueComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Add_edit_banqueComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Add_edit_banqueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
