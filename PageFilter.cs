using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace pagex
{
    #region

    #endregion
    public abstract class PageFilter<T, F> 
        where T : class
        where F : class
    {
        public int page { get; set; }
        public int itensPerPage { get; set; }
        public string orderColumn { get; set; }
        public string orderDirection { get; set; }
        private IEnumerable<T> Order(IEnumerable<T> value)
        {
            System.Reflection.PropertyInfo prop = typeof(T).GetProperty(orderColumn);
            if (orderDirection == "ASC")
                return value.OrderBy(x => prop.GetValue(x, null));
            else if (orderDirection == "DESC")
                return value.OrderByDescending(x => prop.GetValue(x, null));

            return null;
        }
        public virtual FilterResponse<T, F> Paginate(IEnumerable<T> value)
        {
            var res = this.Order(value).ToList();
            var filtered = Filter(res).ToList();
            var complete = filtered.Skip((page - 1) * itensPerPage).Take(itensPerPage).ToList();

            return new FilterResponse<T, F>
            {
                response = complete,
                TotalRowsCount = res.Count(),
                FilteredCount = filtered.Count(),
                RequestCount = complete.Count(),
                pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(filtered.Count() / itensPerPage))),
                itensPerPage = itensPerPage,
                orderColumn = orderColumn,
                orderDirection = orderDirection,
                page = page,
            };
        }
        public virtual IEnumerable<T> Filter(IEnumerable<T> value)
        {
            throw new NotImplementedException("You must implement this method on derived class.");
        }
    }
}
