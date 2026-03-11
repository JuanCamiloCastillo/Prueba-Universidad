import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { switchMap, tap } from 'rxjs/operators';
import { LoginRequest, RegisterRequest, StudentInfo } from '../models/auth.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly TOKEN_KEY = 'se_token';
  private readonly STUDENT_KEY = 'se_student';

  private _currentStudent = signal<StudentInfo | null>(this.loadStoredStudent());
  readonly currentStudent = this._currentStudent.asReadonly();
  readonly isAuthenticated = computed(() => this._currentStudent() !== null);

  constructor(private http: HttpClient, private router: Router) {}

  private loadStoredStudent(): StudentInfo | null {
    try {
      const stored = localStorage.getItem(this.STUDENT_KEY);
      return stored ? JSON.parse(stored) : null;
    } catch {
      return null;
    }
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  /** POST /api/auth/login → { token } — el nombre ya viene en el claim 'name' del JWT */
  login(data: LoginRequest) {
    return this.http
      .post<{ token: string }>(`${environment.apiUrl}/auth/login`, data)
      .pipe(
        tap(res => {
          const decoded = this.decodeToken(res.token);
          if (!decoded) return;

          const student: StudentInfo = {
            id: decoded.id,
            email: decoded.email,
            nombre: decoded.nombre,
            apellido: ''
          };
          this.saveSession(res.token, student);
        })
      );
  }

  /** POST /api/auth/register → { id }  → auto-login para obtener el token */
  register(data: RegisterRequest) {
    return this.http
      .post<{ id: number }>(`${environment.apiUrl}/auth/register`, data)
      .pipe(
        switchMap(() =>
          this.http.post<{ token: string }>(`${environment.apiUrl}/auth/login`, {
            email: data.email,
            contrasena: data.contrasena
          })
        ),
        tap(res => {
          const decoded = this.decodeToken(res.token);
          if (!decoded) return;

          const student: StudentInfo = {
            id: decoded.id,
            email: decoded.email,
            nombre: data.nombre,
            apellido: data.apellido
          };
          this.saveSession(res.token, student);
        })
      );
  }

  /** Decodifica el payload JWT para extraer id, email y nombre (sin verificar firma) */
  private decodeToken(token: string): { id: number; email: string; nombre: string } | null {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return {
        id: Number(payload.studentId ?? payload.sub),
        email: payload.email,
        nombre: payload.name ?? payload.nombre ?? payload.email.split('@')[0]
      };
    } catch {
      return null;
    }
  }

  private saveSession(token: string, student: StudentInfo): void {
    localStorage.setItem(this.TOKEN_KEY, token);
    localStorage.setItem(this.STUDENT_KEY, JSON.stringify(student));
    this._currentStudent.set(student);
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.STUDENT_KEY);
    this._currentStudent.set(null);
    this.router.navigate(['/auth/login']);
  }
}
