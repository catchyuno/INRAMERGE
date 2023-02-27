/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Etat_engagement_periodeComponent } from './etat_engagement_periode.component';

describe('Etat_engagement_periodeComponent', () => {
  let component: Etat_engagement_periodeComponent;
  let fixture: ComponentFixture<Etat_engagement_periodeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Etat_engagement_periodeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Etat_engagement_periodeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
