/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { etat_signatureComponent } from './etat_signature.component';

describe('etat_signatureComponent', () => {
  let component: etat_signatureComponent;
  let fixture: ComponentFixture<etat_signatureComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ etat_signatureComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(etat_signatureComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
