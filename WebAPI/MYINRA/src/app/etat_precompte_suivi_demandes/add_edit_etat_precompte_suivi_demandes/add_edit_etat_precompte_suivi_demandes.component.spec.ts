/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Add_edit_etat_precompte_suivi_demandesComponent } from './add_edit_etat_precompte_suivi_demandes.component';

describe('Add_edit_etat_precompte_suivi_damandesComponent', () => {
  let component: Add_edit_etat_precompte_suivi_demandesComponent;
  let fixture: ComponentFixture<Add_edit_etat_precompte_suivi_demandesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Add_edit_etat_precompte_suivi_demandesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Add_edit_etat_precompte_suivi_demandesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
