import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { EnrollmentService } from '../../core/services/enrollment.service';
import { Enrollment } from '../../core/models/enrollment.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  private authService = inject(AuthService);
  private enrollmentService = inject(EnrollmentService);

  readonly student = this.authService.currentStudent;
  readonly enrollments = signal<Enrollment[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  readonly maxSubjects = 3;
  readonly maxCredits = 9;

  readonly enrolledCount = computed(() => this.enrollments().length);
  readonly totalCredits = computed(() => this.enrolledCount() * 3);
  readonly creditProgress = computed(() =>
    Math.round((this.totalCredits() / this.maxCredits) * 100)
  );

  ngOnInit(): void {
    this.enrollmentService.getMisMatriculas().subscribe({
      next: data => {
        this.enrollments.set(data);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('No se pudieron cargar las matrículas.');
        this.loading.set(false);
      }
    });
  }
}
