/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Show_etat_engagement_suivi_demandesComponent } from './show_etat_engagement_suivi_demandes.component';

describe('Show_etat_engagement_suivi_demandesComponent', () => {
  let component: Show_etat_engagement_suivi_demandesComponent;
  let fixture: ComponentFixture<Show_etat_engagement_suivi_demandesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Show_etat_engagement_suivi_demandesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Show_etat_engagement_suivi_demandesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
