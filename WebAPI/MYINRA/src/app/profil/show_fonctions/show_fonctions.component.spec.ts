/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { show_fonctionsComponent } from './show_fonctions.component';

describe('show_fonctionsComponent', () => {
  let component: show_fonctionsComponent;
  let fixture: ComponentFixture<show_fonctionsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ show_fonctionsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(show_fonctionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
