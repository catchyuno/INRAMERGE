/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { banqueComponent } from './banque.component';

describe('banqueComponent', () => {
  let component: banqueComponent;
  let fixture: ComponentFixture<banqueComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ banqueComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(banqueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
