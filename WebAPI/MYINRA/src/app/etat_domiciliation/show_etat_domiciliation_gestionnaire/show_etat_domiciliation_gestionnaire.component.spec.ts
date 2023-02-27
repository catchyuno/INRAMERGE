/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Show_etat_domiciliation_gestionnaireComponent } from './show_etat_domiciliation_gestionnaire.component';

describe('Show_etat_domiciliation_gestionnaireComponent', () => {
  let component: Show_etat_domiciliation_gestionnaireComponent;
  let fixture: ComponentFixture<Show_etat_domiciliation_gestionnaireComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Show_etat_domiciliation_gestionnaireComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Show_etat_domiciliation_gestionnaireComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
