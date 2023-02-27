/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Show_etat_engagement_periodeComponent } from './show_etat_engagement_periode.component';

describe('Show_etat_engagement_periodeComponent', () => {
  let component: Show_etat_engagement_periodeComponent;
  let fixture: ComponentFixture<Show_etat_engagement_periodeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Show_etat_engagement_periodeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Show_etat_engagement_periodeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
