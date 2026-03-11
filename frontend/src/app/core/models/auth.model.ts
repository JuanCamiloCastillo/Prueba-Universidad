export interface LoginRequest {
  email: string;
  contrasena: string;
}

export interface RegisterRequest {
  nombre: string;
  apellido: string;
  email: string;
  contrasena: string;
}

export interface StudentInfo {
  id: number;
  email: string;
  nombre: string;
  apellido: string;
}
