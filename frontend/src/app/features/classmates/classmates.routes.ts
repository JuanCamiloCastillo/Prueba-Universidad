import { Routes } from '@angular/router';

export const classmatesRoutes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./classmates.component').then(m => m.ClassmatesComponent)
  }
];
