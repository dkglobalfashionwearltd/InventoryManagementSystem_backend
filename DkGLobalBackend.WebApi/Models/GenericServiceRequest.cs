using System.Linq.Expressions;

namespace DkGLobalBackend.WebApi.Models
{
    public class GenericServiceRequest<T>
    {
        public Expression<Func<T, bool>>? Expression { get; set; } = null;
        public string? IncludeProperties { get; set; } = null;
        public bool Tracked { get; set; } = false;
        public CancellationToken CancellationToken { get; set; } = CancellationToken.None;
    }
}
