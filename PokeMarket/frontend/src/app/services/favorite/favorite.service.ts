import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AddFavoriteRequest } from '../../models/DTOs/add-favorite-request';
import { PagedResult } from '../../models/DTOs/pagedresult';
import { CardResponse } from '../../models/DTOs/card-response';

@Injectable({
  providedIn: 'root',
})
export class FavoriteService {
  // añadimos nuestra apiUrl
  private apiUrl = `${environment.apiUrl}/Favorite`;

  constructor(private http: HttpClient) {}

  // Método para comprobar que una carta está en favoritos
  isCardFavorite(
    cardId: number
  ): Observable<{ cardId: number; isFavorite: boolean }> {
    return this.http.get<{ cardId: number; isFavorite: boolean }>(
      `${this.apiUrl}/is-favorite/${cardId}`
    );
  }

  // Método para añadir una carta a favoritos
  addFavorite(
    addFavoriteRequest: AddFavoriteRequest
  ): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(
      `${this.apiUrl}`,
      addFavoriteRequest
    );
  }

  // Método para eliminar una carta de favoritos
  removeFavorite(cardId: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/${cardId}`);
  }

  // Método para obtener mis cartas favoritas
  getMyFavorites(
    page: number = 1,
    collectionName: string,
    pokemonType?: string,
    pokemonName?: string,
    rarity?: string,
    tagName?: string,
    orderBy?: string
  ): Observable<PagedResult<CardResponse>> {
    // para el paginado
    let params = new HttpParams().set('page', page.toString());

    if (pokemonType) params = params.set('pokemonType', pokemonType);
    if (pokemonName) params = params.set('pokemonName', pokemonName);
    if (collectionName) params = params.set('collectionName', collectionName);
    if (rarity) params = params.set('rarity', rarity);
    if (tagName) params = params.set('tagName', tagName);
    if (orderBy) params = params.set('orderBy', orderBy);

    return this.http.get<PagedResult<CardResponse>>(`${this.apiUrl}`, {
      params,
    });
  }

  // Método para obtener los ids de mis cartas favoritas
  getMyFavoriteIds(): Observable<number[]> {
    return this.http.get<number[]>(`${this.apiUrl}/ids`);
  }
}
