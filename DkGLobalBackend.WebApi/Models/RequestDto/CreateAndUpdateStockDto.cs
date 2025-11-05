using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DkGLobalBackend.WebApi.Models.RequestDto
{
    public class CreateAndUpdateStockDto
    {
        [ValidateNever]
        public int ItemId { get; set; }
        [ValidateNever]
        public string ModelNumber { get; set; }
        public int Quantity { get; set; } = 0;
        public string ActionType { get; set; } // "'create', 'deactivate', 'delete', 'plus', or 'minus'."
        [ValidateNever]
        public string ActionBy { get; set; } // "user id"

    }
}
