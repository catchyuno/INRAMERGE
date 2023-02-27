/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { show_sit_familialeComponent } from './show_sit_familiale.component';

describe('show_sit_familialeComponent', () => {
  let component: show_sit_familialeComponent;
  let fixture: ComponentFixture<show_sit_familialeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ show_sit_familialeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(show_sit_familialeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
