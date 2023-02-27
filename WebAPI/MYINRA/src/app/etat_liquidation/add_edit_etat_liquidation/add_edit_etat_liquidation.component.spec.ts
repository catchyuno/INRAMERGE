/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Add_edit_etat_liquidationComponent } from './add_edit_etat_liquidation.component';

describe('Add_edit_etat_liquidationComponent', () => {
  let component: Add_edit_etat_liquidationComponent;
  let fixture: ComponentFixture<Add_edit_etat_liquidationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Add_edit_etat_liquidationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Add_edit_etat_liquidationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
