/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { view_documentComponent } from './view_document.component';

describe('view_documentComponent', () => {
  let component: view_documentComponent;
  let fixture: ComponentFixture<view_documentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ view_documentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(view_documentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
