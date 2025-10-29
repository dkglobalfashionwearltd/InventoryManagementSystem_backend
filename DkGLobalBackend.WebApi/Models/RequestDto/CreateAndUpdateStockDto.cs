namespace DkGLobalBackend.WebApi.Models.RequestDto
{
    public class CreateAndUpdateStockDto
    {
        public string ModelNumber { get; set; }
        public int Quantity { get; set; }
        public string ActionType { get; set; } // "create", "plus", or "minus"
    }
}
