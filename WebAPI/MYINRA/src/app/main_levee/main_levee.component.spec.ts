/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { main_leveeComponent } from './main_levee.component';

describe('Etat_domiciliationComponent', () => {
  let component: main_leveeComponent;
  let fixture: ComponentFixture<main_leveeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ main_leveeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(main_leveeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
