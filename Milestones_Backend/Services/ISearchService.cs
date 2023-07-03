using System.Threading.Tasks;
using System.Collections.Generic;

/// Zum testen eines Generic Service
namespace Milestones.Services
{
    /// <summary>
    /// Defines the rulls, that which methods must be includes in <see cref="SearchService{T}"/> class.
    /// </summary>
    public interface ISearchService<T>
    {
        public Task<List<T>> Search(string subject);
    }
}