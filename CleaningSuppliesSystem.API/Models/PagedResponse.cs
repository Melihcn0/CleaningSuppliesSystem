using System.Text.Json.Serialization;

namespace CleaningSuppliesSystem.API.Models
{
    public class PagedResponse<T>
    {
        [JsonPropertyName("data")]
        public List<T> Data { get; set; }

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }
    }
}
