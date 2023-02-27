/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Show_help_desk_suiviComponent } from './show_help_desk_suivi.component';

describe('Show_help_desk_suiviComponent', () => {
  let component: Show_help_desk_suiviComponent;
  let fixture: ComponentFixture<Show_help_desk_suiviComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Show_help_desk_suiviComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Show_help_desk_suiviComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
