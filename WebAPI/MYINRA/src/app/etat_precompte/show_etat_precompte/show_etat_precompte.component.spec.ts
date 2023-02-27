/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Show_etat_precompteComponent } from './show_etat_precompte.component';

describe('Show_etat_precompteComponent', () => {
  let component: Show_etat_precompteComponent;
  let fixture: ComponentFixture<Show_etat_precompteComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Show_etat_precompteComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Show_etat_precompteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
