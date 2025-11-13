export interface UpdateCardRequest {
  pokemonName: string;
  pokemonType: string;
  rarity: string;
  imageUrl: string;
  collection: string;
  graded: boolean;
  grade: number;
  price: number;
  is_tradeable: boolean;
  tagIds: number[];
}
