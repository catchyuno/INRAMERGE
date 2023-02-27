/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { Add_edit_entete_piedComponent } from './add_edit_entete_pied.component';

describe('Add_edit_entete_piedComponent', () => {
  let component: Add_edit_entete_piedComponent;
  let fixture: ComponentFixture<Add_edit_entete_piedComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ Add_edit_entete_piedComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(Add_edit_entete_piedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
