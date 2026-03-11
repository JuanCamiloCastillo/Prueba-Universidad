import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { EnrollmentService } from '../../core/services/enrollment.service';
import { Enrollment } from '../../core/models/enrollment.model';
import { Classmate } from '../../core/models/classmate.model';

@Component({
  selector: 'app-classmates',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './classmates.component.html',
  styleUrl: './classmates.component.scss'
})
export class ClassmatesComponent implements OnInit {
  private enrollmentService = inject(EnrollmentService);
  private route = inject(ActivatedRoute);

  readonly enrollments = signal<Enrollment[]>([]);
  readonly classmates = signal<Classmate[]>([]);
  readonly loadingEnrollments = signal(true);
  readonly loadingClassmates = signal(false);
  readonly error = signal<string | null>(null);

  selectedSubjectId = '';
  selectedSubjectName = '';

  ngOnInit(): void {
    this.enrollmentService.getMisMatriculas().subscribe({
      next: data => {
        this.enrollments.set(data);
        this.loadingEnrollments.set(false);

        // Pre-seleccionar si viene desde dashboard con ?subjectId=
        const param = this.route.snapshot.queryParamMap.get('subjectId');
        const match = this.enrollments().find(
          e => e.idAsignatura.toString() === param
        );
        if (match) {
          this.selectedSubjectId = match.idAsignatura.toString();
          this.cargarCompaneros(match.idAsignatura);
        }
      },
      error: () => {
        this.error.set('Error al cargar tus materias.');
        this.loadingEnrollments.set(false);
      }
    });
  }

  onSubjectChange(value: string): void {
    this.selectedSubjectId = value;
    this.classmates.set([]);
    this.error.set(null);

    if (!value) {
      this.selectedSubjectName = '';
      return;
    }

    this.cargarCompaneros(Number(value));
  }

  private cargarCompaneros(subjectId: number): void {
    const enrollment = this.enrollments().find(e => e.idAsignatura === subjectId);
    this.selectedSubjectName = enrollment?.nombreAsignatura ?? '';

    this.loadingClassmates.set(true);
    this.enrollmentService.getCompaneros(subjectId).subscribe({
      next: data => {
        this.classmates.set(data);
        this.loadingClassmates.set(false);
      },
      error: () => {
        this.error.set('Error al cargar los compañeros de clase.');
        this.loadingClassmates.set(false);
      }
    });
  }

  getInitials(c: Classmate): string {
    return `${c.nombre[0] ?? ''}${c.apellido[0] ?? ''}`.toUpperCase();
  }
}
