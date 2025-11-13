import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserResponse } from '../../models/DTOs/user-response';
import { PagedResult } from '../../models/DTOs/pagedresult';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private apiUrl = `${environment.apiUrl}/User`;

  constructor(private http: HttpClient) {}

  // Método para obtener al usuario actual
  getUsers(
    page: number = 1,
    username?: string,
    email?: string,
    role?: string,
    orderBy?: string
  ): Observable<PagedResult<UserResponse>> {
    let params = new HttpParams().set('page', page.toString());

    if (username) params = params.set('username', username);
    if (email) params = params.set('email', email);
    if (role) params = params.set('role', role);
    if (orderBy) params = params.set('orderBy', orderBy);

    return this.http.get<PagedResult<UserResponse>>(`${this.apiUrl}`, {
      params,
    });
  }

  // Método para eliminar a un usuario
  deleteUser(userId: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/${userId}`);
  }
}
