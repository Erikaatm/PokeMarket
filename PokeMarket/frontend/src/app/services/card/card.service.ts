import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateCardRequest } from '../../models/DTOs/create-card-request';
import { UpdateCardRequest } from '../../models/DTOs/update-card-request';
import { PagedResult } from '../../models/DTOs/pagedresult';
import { CardResponse } from '../../models/DTOs/card-response';

// Este servicio va a estar disponible en toda la app, en todos los componentes
@Injectable({
  providedIn: 'root',
})
export class CardService {
  // Guardamos la URL base del endpoint de las cartas que tenemos en nuestro back
  private apiUrl = `${environment.apiUrl}/Card`;

  constructor(private http: HttpClient) {}

  // Añadimos los métodos de nuestro back aqui para poder llamarlos desde el front
  // Solo los de el controlador de card
  // Para que funcione correctamente tenemos que crear el modelo Card y los DTOs relacionados con este controlador

  // Método para crear una carta, necesitamos crear el DTO createCardRequest
  createCard(request: CreateCardRequest): Observable<CardResponse> {
    return this.http.post<CardResponse>(`${this.apiUrl}`, request);
  }

  // Método para obtener todas las cartas
  getCards(
    page: number = 1,
    pokemonType?: string,
    collection?: string,
    pokemonName?: string,
    rarity?: string,
    tagName?: string,
    orderBy?: string
  ): Observable<PagedResult<CardResponse>> {
    // para el paginado
    let params = new HttpParams().set('page', page.toString());

    if (pokemonType) params = params.set('pokemonType', pokemonType);
    if (collection) params = params.set('collection', collection);
    if (pokemonName) params = params.set('pokemonName', pokemonName);
    if (rarity) params = params.set('rarity', rarity);
    if (tagName) params = params.set('tagName', tagName);
    if (orderBy) params = params.set('orderBy', orderBy);

    return this.http.get<PagedResult<CardResponse>>(`${this.apiUrl}`, {
      params,
    });
  }

  // Método para obtener una carta por id
  getCardById(id: number): Observable<CardResponse> {
    return this.http.get<CardResponse>(`${this.apiUrl}/${id}`);
  }

  // Método para editar una carta
  updateCard(id: number, request: UpdateCardRequest): Observable<CardResponse> {
    return this.http.put<CardResponse>(`${this.apiUrl}/${id}`, request);
  }

  // Método para eliminar una carta
  deleteCard(id: number): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/${id}`);
  }

  // Método para obtener las cartas del usuario logeado
  getMyCards(
    page: number = 1,
    pokemonType?: string,
    collection?: string,
    pokemonName?: string,
    rarity?: string,
    tagName?: string,
    orderBy?: string
  ): Observable<PagedResult<CardResponse>> {
    // para el paginado
    let params = new HttpParams().set('page', page.toString());

    if (pokemonType) params = params.set('pokemonType', pokemonType);
    if (collection) params = params.set('collection', collection);
    if (pokemonName) params = params.set('pokemonName', pokemonName);
    if (rarity) params = params.set('rarity', rarity);
    if (tagName) params = params.set('tagName', tagName);
    if (orderBy) params = params.set('orderBy', orderBy);

    return this.http.get<PagedResult<CardResponse>>(`${this.apiUrl}/my-cards`, {
      params,
    });
  }

  // Método para obtener una carta por id del usuario logeado
  getMyCardById(id: number): Observable<CardResponse> {
    return this.http.get<CardResponse>(`${this.apiUrl}/my-cards/${id}`);
  }

  // Método para obtener todas las cartas de un usuario en específico
  getCardsByUserId(userId: number): Observable<CardResponse[]> {
    return this.http.get<CardResponse[]>(`${this.apiUrl}/user/${userId}`);
  }

  // Método para obtener las cartas segun su colecciión
  getCardsByCollection(
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
    if (rarity) params = params.set('rarity', rarity);
    if (tagName) params = params.set('tagName', tagName);
    if (orderBy) params = params.set('orderBy', orderBy);

    const encodedCollection = encodeURIComponent(collectionName);

    return this.http.get<PagedResult<CardResponse>>(
      `${this.apiUrl}/collection/${encodedCollection}`,
      { params }
    );
  }

  // Método para añadir imagens
  uploadImage(file: File): Observable<{ imageUrl: string }> {
    // Cremaos un objeto FormData para enviar archivos
    const formData = new FormData();

    // Le añadimos el archivo con clave file igual que en el back, tiene que coincidir
    formData.append('file', file);

    // Llamamos al endpoint
    return this.http.post<{ imageUrl: string }>(
      `${this.apiUrl}/upload-image`,
      formData
    );
  }
}
