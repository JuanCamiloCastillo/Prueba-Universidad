export interface Enrollment {
  id: number;
  idAsignatura: number;
  nombreAsignatura: string;
  nombreProfesor: string;
  fechaInscripcion: string;
}

export interface EnrollmentRequest {
  idAsignatura: number;
}
