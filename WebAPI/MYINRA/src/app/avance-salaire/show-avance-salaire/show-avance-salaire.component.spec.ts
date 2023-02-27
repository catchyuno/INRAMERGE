import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ShowAvanceSalaireComponent } from './show-avance-salaire.component';

describe('ShowAvanceSalaireComponent', () => {
  let component: ShowAvanceSalaireComponent;
  let fixture: ComponentFixture<ShowAvanceSalaireComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ShowAvanceSalaireComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ShowAvanceSalaireComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
