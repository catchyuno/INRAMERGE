/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Etat_precompte_suivi_demandesComponent } from './etat_precompte_suivi_demandes.component';

describe('Etat_precompte_suivi_demandesComponent', () => {
  let component: Etat_precompte_suivi_demandesComponent;
  let fixture: ComponentFixture<Etat_precompte_suivi_demandesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Etat_precompte_suivi_demandesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Etat_precompte_suivi_demandesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
