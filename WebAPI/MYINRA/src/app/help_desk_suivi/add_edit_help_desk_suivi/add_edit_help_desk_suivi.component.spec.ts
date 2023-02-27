/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Add_edit_help_desk_suiviComponent } from './add_edit_help_desk_suivi.component';

describe('Add_edit_help_desk_suiviComponent', () => {
  let component: Add_edit_help_desk_suiviComponent;
  let fixture: ComponentFixture<Add_edit_help_desk_suiviComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Add_edit_help_desk_suiviComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Add_edit_help_desk_suiviComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
