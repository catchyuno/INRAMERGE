/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { show_gradesComponent } from './show_grades.component';

describe('show_gradesComponent', () => {
  let component: show_gradesComponent;
  let fixture: ComponentFixture<show_gradesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ show_gradesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(show_gradesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
