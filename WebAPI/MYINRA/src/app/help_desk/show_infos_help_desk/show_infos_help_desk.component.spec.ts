/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { show_infos_help_deskComponent } from './show_infos_help_desk.component';

describe('show_infos_help_deskComponent', () => {
  let component: show_infos_help_deskComponent;
  let fixture: ComponentFixture<show_infos_help_deskComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ show_infos_help_deskComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(show_infos_help_deskComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
