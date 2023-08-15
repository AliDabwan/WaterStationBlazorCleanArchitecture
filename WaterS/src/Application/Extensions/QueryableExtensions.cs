using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterS.Application.Exceptions;
using WaterS.Application.Specifications.Base;
using WaterS.Domain.Contracts;
using WaterS.Shared.Wrapper;

namespace WaterS.Application.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PaginatedResult<T>> ToPaginatedListAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize) where T : class
        {
            if (source == null) throw new ApiException();
            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            int count = await source.CountAsync();

            pageSize = pageSize == 0 ? count : pageSize;// if page size set to zero returns all rows  @ali_ad007

            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            List<T> items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
         
            return PaginatedResult<T>.Success(items, count, pageNumber, pageSize);
        }
      
        public static async Task<PaginatedResult<T>> allAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize) where T : class
        {
            if (source == null) throw new ApiException();
            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            pageSize = pageSize == 0 ? 100 : pageSize;
            int count = await source.CountAsync();
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            List<T> items = await source.ToListAsync();
            return PaginatedResult<T>.Success(items, count, pageNumber, pageSize);
        }
        public static async Task<PaginatedResult<T>> allAsync<T>(this IQueryable<T> source) where T : class
        {
            if (source == null) throw new ApiException();
          
            List<T> items = await source.ToListAsync();
            return PaginatedResult<T>.Success(items);
        }

        public static IQueryable<T> Specify<T>(this IQueryable<T> query, ISpecification<T> spec) where T : class, IEntity
        {
            var queryableResultWithIncludes = spec.Includes
                .Aggregate(query,
                    (current, include) => current.Include(include));
            var secondaryResult = spec.IncludeStrings
                .Aggregate(queryableResultWithIncludes,
                    (current, include) => current.Include(include));
            return secondaryResult.Where(spec.Criteria);
        }
    }
}