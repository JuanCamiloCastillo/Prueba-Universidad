import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Enrollment, EnrollmentRequest } from '../models/enrollment.model';
import { Classmate } from '../models/classmate.model';

@Injectable({ providedIn: 'root' })
export class EnrollmentService {
  private readonly apiUrl = `${environment.apiUrl}/inscripciones`;
  private readonly asignaturasUrl = `${environment.apiUrl}/asignaturas`;

  constructor(private http: HttpClient) {}

  getMisMatriculas() {
    return this.http.get<Enrollment[]>(this.apiUrl);
  }

  matricular(data: EnrollmentRequest) {
    return this.http.post<{ mensaje: string }>(this.apiUrl, data);
  }

  cancelarMatricula(enrollmentId: number) {
    return this.http.delete<void>(`${this.apiUrl}/${enrollmentId}`);
  }

  getCompaneros(idAsignatura: number) {
    return this.http.get<Classmate[]>(`${this.asignaturasUrl}/${idAsignatura}/companeros`);
  }
}
