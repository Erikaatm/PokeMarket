export interface UserResponse {
  id: number;
  username: string;
  firstName: string;
  lastName1: string;
  lastName2?: string;
  emailAddress: string;
  phoneNumber?: string;
  address?: string;
  createdAt: string;         
  lastUpdatedAt: string;
  role: string;
}
