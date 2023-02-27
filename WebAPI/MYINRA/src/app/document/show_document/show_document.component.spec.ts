/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Show_documentComponent } from './show_document.component';

describe('Show_documentComponent', () => {
  let component: Show_documentComponent;
  let fixture: ComponentFixture<Show_documentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Show_documentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Show_documentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
