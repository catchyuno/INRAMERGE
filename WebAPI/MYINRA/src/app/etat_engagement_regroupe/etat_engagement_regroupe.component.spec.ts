/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Etat_engagement_regroupeComponent } from './etat_engagement_regroupe.component';

describe('Etat_engagement_regroupeComponent', () => {
  let component: Etat_engagement_regroupeComponent;
  let fixture: ComponentFixture<Etat_engagement_regroupeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Etat_engagement_regroupeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Etat_engagement_regroupeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
