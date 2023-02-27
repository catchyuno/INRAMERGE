/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Show_infos_banqueComponent } from './show_infos_banque.component';

describe('Show_infos_banqueComponent', () => {
  let component: Show_infos_banqueComponent;
  let fixture: ComponentFixture<Show_infos_banqueComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Show_infos_banqueComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Show_infos_banqueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
