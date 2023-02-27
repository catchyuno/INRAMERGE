import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MarcheBcComponent } from './marche-bc.component';

describe('MarcheBcComponent', () => {
  let component: MarcheBcComponent;
  let fixture: ComponentFixture<MarcheBcComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MarcheBcComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MarcheBcComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
