/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Add_edit_salaire_rubriqueComponent } from './add_edit_salaire_rubrique.component';

describe('Add_edit_salaire_rubriqueComponent', () => {
  let component: Add_edit_salaire_rubriqueComponent;
  let fixture: ComponentFixture<Add_edit_salaire_rubriqueComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
       declarations: [ Add_edit_salaire_rubriqueComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Add_edit_salaire_rubriqueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
