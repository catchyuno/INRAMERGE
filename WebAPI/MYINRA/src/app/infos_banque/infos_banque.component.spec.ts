/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { infos_banqueComponent } from './infos_banque.component';

describe('infos_banqueComponent', () => {
  let component: infos_banqueComponent;
  let fixture: ComponentFixture<infos_banqueComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ infos_banqueComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(infos_banqueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
