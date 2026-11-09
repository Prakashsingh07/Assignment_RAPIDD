using System.Collections.Generic;
using System.Threading.Tasks;
using Prakash_Singh_CSharp_Assignment.Models;

namespace Prakash_Singh_CSharp_Assignment.Services
{
    public interface IApiService
    {
        /// <summary>
        /// Fetches time entries from the specified REST API endpoint, with fallback support.
        /// </summary>
        /// <param name="endpointUrl">The Azure REST API endpoint URL.</param>
        /// <returns>A list of TimeEntry records.</returns>
        Task<List<TimeEntry>> FetchTimeEntriesAsync(string endpointUrl);
    }
}
