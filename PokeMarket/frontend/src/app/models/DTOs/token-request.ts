export interface TokenRequest {
  token: string;
  message: string;
  user: {
    id: number;
    username: string;
    emailAddress: string;
  };
}
