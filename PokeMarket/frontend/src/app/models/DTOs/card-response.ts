import { TagResponse } from './tag-response';

export interface CardResponse {
  id: number;
  pokemonName: string;
  pokemonType: string;
  rarity: string;
  imageUrl: string;
  collection: string;
  graded: boolean;
  grade: number;
  gradeRequested: boolean;
  gradeStatus: string;
  price: number;
  is_tradeable: boolean;
  createdAt: Date;
  lastUpdatedAt: Date;
  userId: number;
  username?: string;
  tags?: TagResponse[];
}