export interface PagedResult<T> {
  currentPage: number;
  pageSize: number;
  totalPages: number;
  totalCount: number;
  items: T[];
}
