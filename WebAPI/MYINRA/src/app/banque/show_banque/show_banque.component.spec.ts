/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Show_banqueComponent } from './show_banque.component';

describe('Show_banqueComponent', () => {
  let component: Show_banqueComponent;
  let fixture: ComponentFixture<Show_banqueComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Show_banqueComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Show_banqueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
