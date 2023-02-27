/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { main_levee_suivi_demandesComponent } from './main_levee_suivi_demandes.component';

describe('main_levee_suivi_demandesComponent', () => {
  let component: main_levee_suivi_demandesComponent;
  let fixture: ComponentFixture<main_levee_suivi_demandesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ main_levee_suivi_demandesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(main_levee_suivi_demandesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
