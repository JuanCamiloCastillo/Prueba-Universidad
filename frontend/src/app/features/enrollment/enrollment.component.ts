import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { forkJoin } from 'rxjs';
import { SubjectService } from '../../core/services/subject.service';
import { EnrollmentService } from '../../core/services/enrollment.service';
import { Subject } from '../../core/models/subject.model';

@Component({
  selector: 'app-enrollment',
  standalone: true,
  imports: [],
  templateUrl: './enrollment.component.html',
  styleUrl: './enrollment.component.scss'
})
export class EnrollmentComponent implements OnInit {
  private subjectService = inject(SubjectService);
  private enrollmentService = inject(EnrollmentService);

  readonly subjects = signal<Subject[]>([]);
  readonly loading = signal(true);
  readonly actionLoading = signal<number | null>(null);
  readonly error = signal<string | null>(null);
  readonly successMsg = signal<string | null>(null);

  readonly enrolledSubjects = computed(() => this.subjects().filter(s => s.isEnrolled));
  readonly enrolledCount = computed(() => this.enrolledSubjects().length);
  readonly canEnrollMore = computed(() => this.enrolledCount() < 3);

  ngOnInit(): void {
    this.cargarMaterias();
  }

  private cargarMaterias(): void {
    this.loading.set(true);
    forkJoin({
      subjects: this.subjectService.getAll(),
      enrollments: this.enrollmentService.getMisMatriculas()
    }).subscribe({
      next: ({ subjects, enrollments }) => {
        // Mapeamos la lista de inscripciones para enriquecer cada materia
        const enrollmentMap = new Map(enrollments.map(e => [e.idAsignatura, e.id]));
        this.subjects.set(
          subjects.map(s => ({
            ...s,
            isEnrolled: enrollmentMap.has(s.id),
            enrollmentId: enrollmentMap.get(s.id),
            cuposDisponibles: s.capacidadMaxima - s.cantidadInscritos
          }))
        );
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Error al cargar las materias. Intenta de nuevo.');
        this.loading.set(false);
      }
    });
  }

  matricular(subject: Subject): void {
    this.actionLoading.set(subject.id);
    this.error.set(null);
    this.successMsg.set(null);

    this.enrollmentService.matricular({ idAsignatura: subject.id }).subscribe({
      next: res => {
        this.successMsg.set(res.mensaje);
        this.cargarMaterias();
        this.actionLoading.set(null);
      },
      error: err => {
        this.error.set(err.error?.error ?? 'Error al matricular. Intenta de nuevo.');
        this.actionLoading.set(null);
      }
    });
  }

  cancelar(subject: Subject): void {
    if (!subject.enrollmentId) return;

    this.actionLoading.set(subject.id);
    this.error.set(null);
    this.successMsg.set(null);

    this.enrollmentService.cancelarMatricula(subject.enrollmentId).subscribe({
      next: () => {
        this.successMsg.set(`Matrícula en "${subject.nombre}" cancelada.`);
        this.cargarMaterias();
        this.actionLoading.set(null);
      },
      error: () => {
        this.error.set('Error al cancelar la matrícula. Intenta de nuevo.');
        this.actionLoading.set(null);
      }
    });
  }
}
