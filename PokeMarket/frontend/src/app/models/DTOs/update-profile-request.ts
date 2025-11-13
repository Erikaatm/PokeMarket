export interface UpdateProfileRequest {
  userName: string;
  firstName?: string;
  lastName1?: string;
  lastName2?: string;
  phoneNumber?: string;
  address?: string;

  currentEmail: string;
  currentPassword: string;
}