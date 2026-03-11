export interface Subject {
  id: number;
  nombre: string;
  creditos: number;
  nombreProfesor: string;
  capacidadMaxima: number;
  cantidadInscritos: number;
  // campos calculados en el cliente
  isEnrolled?: boolean;
  enrollmentId?: number;
  cuposDisponibles?: number;
}
