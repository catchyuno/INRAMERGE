/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { help_desk_suiviComponent } from './help_desk_suivi.component';

describe('help_desk_suiviComponent', () => {
  let component: help_desk_suiviComponent;
  let fixture: ComponentFixture<help_desk_suiviComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ help_desk_suiviComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(help_desk_suiviComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
