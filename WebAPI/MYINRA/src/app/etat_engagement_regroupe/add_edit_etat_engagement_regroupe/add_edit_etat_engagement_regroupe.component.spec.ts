/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Add_edit_etat_engagement_regroupeComponent } from './add_edit_etat_engagement_regroupe.component';

describe('Add_edit_etat_engagement_regroupeComponent', () => {
  let component: Add_edit_etat_engagement_regroupeComponent;
  let fixture: ComponentFixture<Add_edit_etat_engagement_regroupeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Add_edit_etat_engagement_regroupeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Add_edit_etat_engagement_regroupeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
