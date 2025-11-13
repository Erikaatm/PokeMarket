import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResult } from '../../models/DTOs/pagedresult';
import { GradingResponse } from '../../models/DTOs/grading-response';
import { GradeCardRequest } from '../../models/DTOs/grade-card-request';
import { CardResponse } from '../../models/DTOs/card-response';

@Injectable({
  providedIn: 'root',
})
export class GradingService {
  private apiUrl = `${environment.apiUrl}/Grading`;

  constructor(private http: HttpClient) {}

  // Método para obtener todos los gradeos con filtros y paginación
  getAllGradings(
    page: number = 1,
    pokemonType?: string,
    collection?: string,
    pokemonName?: string,
    rarity?: string,
    orderBy?: string,
    gradedBy?: string
  ): Observable<PagedResult<GradingResponse>> {
    // para el paginado
    let params = new HttpParams().set('page', page.toString());

    if (pokemonType) params = params.set('pokemonType', pokemonType);
    if (collection) params = params.set('collection', collection);
    if (pokemonName) params = params.set('pokemonName', pokemonName);
    if (rarity) params = params.set('rarity', rarity);
    if (orderBy) params = params.set('orderBy', orderBy);
    if (gradedBy) params = params.set('gradedBy', gradedBy);

    return this.http.get<PagedResult<GradingResponse>>(`${this.apiUrl}`, {
      params,
    });
  }

  // Método para obtener un gradeo por su id
  getGradingById(id: number): Observable<GradingResponse> {
    return this.http.get<GradingResponse>(`${this.apiUrl}/${id}`);
  }

  // Método para enviar una solicitud de gradeo (usuario)
  requestGrading(cardId: number): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/request/${cardId}`, {});
  }

  // Método para obtener cartas propias aún sin solicitar gradeo
  getMyUngradedCards(): Observable<CardResponse[]> {
    return this.http.get<CardResponse[]>(`${this.apiUrl}/my-ungraded-cards`);
  }

  // Método para aceptar un gradeo
  gradeCard(request: GradeCardRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(this.apiUrl, request);
  }

  // Método para rechazar un gradeo (admins o graders)
  rejectGrading(cardId: number): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(
      `${this.apiUrl}/reject/${cardId}`,
      {}
    );
  }

  // Método para eliminar un gradeo (admins o graders)
  deleteGrading(id: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/${id}`);
  }

  // Método para obtener los gradeos solicitados
  getRequestedCards(
    page: number = 1,
    collectionName: string,
    pokemonType?: string,
    pokemonName?: string,
    rarity?: string,
    orderBy?: string
  ): Observable<PagedResult<CardResponse>> {
    // para el paginado
    let params = new HttpParams().set('page', page.toString());

    if (pokemonType) params = params.set('pokemonType', pokemonType);
    if (pokemonName) params = params.set('pokemonName', pokemonName);
    if (collectionName) params = params.set('collectionName', collectionName);
    if (rarity) params = params.set('rarity', rarity);
    if (orderBy) params = params.set('orderBy', orderBy);

    return this.http.get<PagedResult<CardResponse>>(
      `${this.apiUrl}/requested`,
      { params }
    );
  }
}
