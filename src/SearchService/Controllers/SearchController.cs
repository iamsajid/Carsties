using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Controllers
{
    [Route("api/search")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Item>>> SearchItems([FromQuery] SearchParams searchParams)
        {
            var query = DB.PagedSearch<Item, Item>();

            if (!string.IsNullOrEmpty(searchParams.searchTerm))
            {
                query.Match(Search.Full, searchParams.searchTerm).SortByTextScore();
            }

            query.PageNumber(searchParams.PageNumber);
            query.PageSize(searchParams.PageSize);

            query = searchParams.OrderBy switch 
            {
                "make" => query.Sort(x => x.Ascending(i => i.Make)),
                "new" => query.Sort(x => x.Descending(i => i.CreatedAt)),
                _ => query.Sort(x => x.Ascending(i => i.AuctionEnd))
            };

            query = searchParams.FilterBy switch 
            {
                "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
                "endingsoon" => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6) 
                        && x.AuctionEnd > DateTime.UtcNow),
                _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
            };

            if (!string.IsNullOrEmpty(searchParams.Seller))
            {
                query.Match(x => x.Seller == searchParams.Seller);
            }

            if (!string.IsNullOrEmpty(searchParams.Winner))
            {
                query.Match(x => x.Winner == searchParams.Winner);
            }
            var result = await query.ExecuteAsync();

            return Ok(new
            {
                results = result.Results,
                pageCount = result.PageCount,
                totalCount = result.TotalCount
            });
        }
    }
}
