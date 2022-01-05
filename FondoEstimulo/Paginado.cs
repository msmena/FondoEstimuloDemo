using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FondoEstimulo
{
    public class Paginado<T> : List<T>
    {
        #region Constructors

        public Paginado(List<T> items, int count, int pageIndex, int pageSize)
        {
            if (pageIndex > 0 && pageIndex < count)
            {
                PageIndex = pageIndex;
            }
            else if (pageIndex > count)
            {
                PageIndex = count;
            }
            
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            RecordsFrom = items.Count > 0 ? 1 : 0;
            RecordsFrom += ((pageIndex - 1) * pageSize);
            RecordsTo = RecordsFrom + (items.Count - 1);
            RecordsTotal = count;
            this.AddRange(items);

            int maxPages = 9;
            if (TotalPages <= maxPages)
            {
                StartPage = 1;
                EndPage = TotalPages;
            }
            else
            {
                int maxPagesBeforeCurrentPage = (int)Math.Floor((decimal)maxPages / (decimal)2);
                int maxPagesAfterCurrentPage = (int)Math.Ceiling((decimal)maxPages / (decimal)2) - 1;
                if (pageIndex <= maxPagesBeforeCurrentPage)
                {
                    StartPage = 1;
                    EndPage = maxPages;
                }
                else if (pageIndex + maxPagesAfterCurrentPage > TotalPages)
                {
                    StartPage = TotalPages - maxPages + 1;
                    EndPage = TotalPages;
                }
                else
                {
                    StartPage = pageIndex - maxPagesBeforeCurrentPage;
                    EndPage = pageIndex + maxPagesAfterCurrentPage;
                }
            }
        }

        #endregion Constructors

        #region Properties

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public int PageIndex { get; private set; } = 1;
        public int RecordsFrom { get; private set; }
        public int RecordsTo { get; private set; }
        public int RecordsTotal { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }

        #endregion Properties

        #region Methods

        public static async Task<Paginado<T>> CreateAsync(
            IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip(
                (pageIndex - 1) * pageSize)
                .Take(pageSize).ToListAsync();
            return new Paginado<T>(items, count, pageIndex, pageSize);
        }

        #endregion Methods
    }
}