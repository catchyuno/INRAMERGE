/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import {cinComponent } from './cin.component';

describe('cinComponent', () => {
  let component: cinComponent;
  let fixture: ComponentFixture<cinComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ cinComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(cinComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
