/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Show_help_deskComponent } from './show_help_desk.component';

describe('Show_help_deskComponent', () => {
  let component: Show_help_deskComponent;
  let fixture: ComponentFixture<Show_help_deskComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Show_help_deskComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Show_help_deskComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
