namespace KshatriyaSportsFoundations.API.Models.Dtos
{
    public class GenericAPIResponseDto
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }
        public int? TotalRecordsCount { get; set; } = null;
    }
}
