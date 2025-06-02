namespace PokeMarket.Models.DTOs
{
    public class PagedResult<T>
    {
        public int CurrentPage { get; set; } // Pagina en la que nos encontramos

        public int TotalPages { get; set; } // Total de las páginas que hay disponibles

        public int PageSize { get; set; } // Cantidad de resultados que hay por página

        public int TotalCount { get; set; } // Número total de resultados de la búsqueda, en este caso cartas

        public List<T> Items { get; set; } = new(); // La lista de los objetos de la página en la que estas
    }
}
