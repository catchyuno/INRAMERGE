import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditAvanceSalaireComponent } from './add-edit-avance-salaire.component';

describe('AddEditAvanceSalaireComponent', () => {
  let component: AddEditAvanceSalaireComponent;
  let fixture: ComponentFixture<AddEditAvanceSalaireComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddEditAvanceSalaireComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddEditAvanceSalaireComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
