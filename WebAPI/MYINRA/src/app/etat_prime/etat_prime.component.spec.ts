/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Etat_primeComponent } from './etat_prime.component';

describe('Etat_primeComponent', () => {
  let component: Etat_primeComponent;
  let fixture: ComponentFixture<Etat_primeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Etat_primeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Etat_primeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
