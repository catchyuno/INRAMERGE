/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Show_etat_revenuComponent } from './show_etat_revenu.component';

describe('Show_etat_revenuComponent', () => {
  let component: Show_etat_revenuComponent;
  let fixture: ComponentFixture<Show_etat_revenuComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Show_etat_revenuComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Show_etat_revenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
