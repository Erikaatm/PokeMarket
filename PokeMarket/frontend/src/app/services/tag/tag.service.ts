import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { PagedResult } from '../../models/DTOs/pagedresult';
import { TagResponse } from '../../models/DTOs/tag-response';
import { CardResponse } from '../../models/DTOs/card-response';

@Injectable({
  providedIn: 'root'
})
export class TagService {
  private apiUrl = `${environment.apiUrl}/Tag`;

  constructor(private http: HttpClient) { }

  // Obtener todos los tags con filtros y paginación
  getAllTags(
    page: number = 1,
    searchTag?: string,
    orderBy?: string
  ): Observable<PagedResult<TagResponse>> {
    let params = new HttpParams().set('page', page);

    if (searchTag) params = params.set('searchTag', searchTag);
    if (orderBy) params = params.set('orderBy', orderBy);

    return this.http.get<PagedResult<TagResponse>>(this.apiUrl, { params });
  }

  // Crear un nuevo tag (solo admins)
  createTag(tag: { name: string }): Observable<{ message: string; tag: TagResponse }> {
    return this.http.post<{ message: string; tag: TagResponse }>(this.apiUrl, tag);
  }

  // Eliminar tag por ID (solo admins)
  deleteTag(id: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/${id}`);
  }

  // Obtener un tag por ID (con cartas relacionadas y filtros)
  getTagById(
    id: number,
    page: number = 1,
    pokemonType?: string,
    collection?: string,
    pokemonName?: string,
    rarity?: string,
    orderBy?: string
  ): Observable<PagedResult<CardResponse>> {
    let params = new HttpParams().set('page', page);

    if (pokemonType) params = params.set('pokemonType', pokemonType);
    if (collection) params = params.set('collection', collection);
    if (pokemonName) params = params.set('pokemonName', pokemonName);
    if (rarity) params = params.set('rarity', rarity);
    if (orderBy) params = params.set('orderBy', orderBy);

    return this.http.get<PagedResult<CardResponse>>(`${this.apiUrl}/${id}`, { params });
  }
}
