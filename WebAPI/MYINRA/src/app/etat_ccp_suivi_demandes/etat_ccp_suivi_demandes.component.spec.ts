/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Etat_ccp_suivi_demandesComponent } from './etat_ccp_suivi_demandes.component';

describe('Etat_ccp_suivi_demandesComponent', () => {
  let component: Etat_ccp_suivi_demandesComponent;
  let fixture: ComponentFixture<Etat_ccp_suivi_demandesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Etat_ccp_suivi_demandesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Etat_ccp_suivi_demandesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
