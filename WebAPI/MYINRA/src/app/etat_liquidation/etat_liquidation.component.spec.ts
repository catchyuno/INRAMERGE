/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Etat_liquidationComponent } from './etat_liquidation.component';

describe('Etat_liquidationComponent', () => {
  let component: Etat_liquidationComponent;
  let fixture: ComponentFixture<Etat_liquidationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Etat_liquidationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Etat_liquidationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
