export interface GradingResponse {
  id: number;
  cardId: number; 
  pokemonName: string; 
  collection: string; 
  rarity: string;
  imageUrl: string; 
  price: number; 

  grade: number;
  gradedByUsername: string; 
  gradeStatus: string;
  createdByUsername: string;

  gradedAt: string;
}
