/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import {show_infos_help_desk_suiviComponent} from './show_infos_help_desk_suivi.component';

describe('show_infos_help_desk_suiviComponent', () => {
  let component: show_infos_help_desk_suiviComponent;
  let fixture: ComponentFixture<show_infos_help_desk_suiviComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ show_infos_help_desk_suiviComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(show_infos_help_desk_suiviComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
