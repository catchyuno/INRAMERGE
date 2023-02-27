/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { show_affectationsComponent } from './show_affectations.component';

describe('show_affectationsComponent', () => {
  let component: show_affectationsComponent;
  let fixture: ComponentFixture<show_affectationsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ show_affectationsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(show_affectationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
