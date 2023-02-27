/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Show_cinComponent } from './show_cin.component';

describe('Show_etat_engagementComponent', () => {
  let component: Show_cinComponent;
  let fixture: ComponentFixture<Show_cinComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Show_cinComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Show_cinComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
