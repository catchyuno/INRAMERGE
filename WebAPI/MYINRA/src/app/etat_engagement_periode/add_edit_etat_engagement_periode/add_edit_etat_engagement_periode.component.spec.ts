/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Add_edit_etat_engagement_periodeComponent } from './add_edit_etat_engagement_periode.component';

describe('Add_edit_etat_engagement_periodeComponent', () => {
  let component: Add_edit_etat_engagement_periodeComponent;
  let fixture: ComponentFixture<Add_edit_etat_engagement_periodeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Add_edit_etat_engagement_periodeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Add_edit_etat_engagement_periodeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
