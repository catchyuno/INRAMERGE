/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { help_deskComponent } from './help_desk.component';

describe('help_deskComponent', () => {
  let component: help_deskComponent;
  let fixture: ComponentFixture<help_deskComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ help_deskComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(help_deskComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
