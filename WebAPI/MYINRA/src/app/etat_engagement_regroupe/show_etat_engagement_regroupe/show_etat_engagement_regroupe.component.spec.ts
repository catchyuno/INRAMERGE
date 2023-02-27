/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Show_etat_engagement_regroupeComponent } from './show_etat_engagement_regroupe.component';

describe('Show_etat_engagement_regroupeComponent', () => {
  let component: Show_etat_engagement_regroupeComponent;
  let fixture: ComponentFixture<Show_etat_engagement_regroupeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Show_etat_engagement_regroupeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Show_etat_engagement_regroupeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
