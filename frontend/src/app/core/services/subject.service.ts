import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Subject } from '../models/subject.model';

@Injectable({ providedIn: 'root' })
export class SubjectService {
  private readonly apiUrl = `${environment.apiUrl}/asignaturas`;

  constructor(private http: HttpClient) {}

  getAll() {
    return this.http.get<Subject[]>(this.apiUrl);
  }
}
