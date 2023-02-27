/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { banque_codeComponent } from './banque_code.component';

describe('banqueComponent', () => {
  let component: banque_codeComponent;
  let fixture: ComponentFixture<banque_codeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ banque_codeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(banque_codeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
