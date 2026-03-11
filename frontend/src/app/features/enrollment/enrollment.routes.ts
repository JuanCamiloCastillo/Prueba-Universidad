import { Routes } from '@angular/router';

export const enrollmentRoutes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./enrollment.component').then(m => m.EnrollmentComponent)
  }
];
