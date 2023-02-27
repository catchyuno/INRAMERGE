/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Add_edit_etat_precompteComponent } from './add_edit_etat_precompte.component';

describe('Add_edit_etat_precompteComponent', () => {
  let component: Add_edit_etat_precompteComponent;
  let fixture: ComponentFixture<Add_edit_etat_precompteComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Add_edit_etat_precompteComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Add_edit_etat_precompteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
