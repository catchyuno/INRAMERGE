/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { profilComponent } from './profil.component';

describe('profilComponent', () => {
  let component: profilComponent;
  let fixture: ComponentFixture<profilComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ profilComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(profilComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
