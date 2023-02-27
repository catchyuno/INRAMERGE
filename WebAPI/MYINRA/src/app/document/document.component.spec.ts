/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { documentComponent } from './document.component';

describe('documentComponent', () => {
  let component: documentComponent;
  let fixture: ComponentFixture<documentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ documentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(documentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
