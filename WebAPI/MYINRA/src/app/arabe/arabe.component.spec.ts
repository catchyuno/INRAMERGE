/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { arabeComponent } from './arabe.component';

describe('arabeComponent', () => {
  let component: arabeComponent;
  let fixture: ComponentFixture<arabeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ arabeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(arabeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
