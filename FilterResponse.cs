using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace pagex
{
    public class FilterResponse<T, F> where T : class where F : class
    {
        /// <summary>
        /// Get the response of filtering/pagination.
        /// </summary>
        public IEnumerable<T> response { get; set; }
        
        /// <summary>
        /// Actual page.
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// Count of itens per page returned on response.
        /// </summary>
        public int itensPerPage { get; set; }
        /// <summary>
        /// Count of pages considering itensPerPage and FilteredCount
        /// </summary>
        public int pageCount { get; set; }
        /// <summary>
        /// Return count of filtered itens. Case has no filter, will be the same of TotalRowsCount
        /// </summary>
        public int FilteredCount { get; set; }
        /// <summary>
        /// Count of response itens
        /// </summary>
        public int RequestCount { get; set; }
        /// <summary>
        /// Count of all items of repository.
        /// </summary>
        public int TotalRowsCount { get; set; }
        /// <summary>
        /// Column to order. Must be the same name of the class.
        /// </summary>
        public string orderColumn { get; set; }
        /// <summary>
        /// Order direction (ASC/DESC)
        /// </summary>
        public string orderDirection { get; set; }
        public List<PageModel> Pages
        {
            get
            {
                List<PageModel> pm = new List<PageModel>();
                var pages = new Pager(FilteredCount, page, itensPerPage);

                if(pages.CurrentPage > 1) {
                    pm.Add(new PageModel
                    {
                        Active = false,
                        Label = "<<",
                        PageNumber = pages.StartPage
                    });
                    pm.Add(new PageModel
                    {
                        Active = false,
                        Label = "<",
                        PageNumber = pages.CurrentPage - 1
                    });
                }

                for (int i = pages.StartPage; i <= pages.EndPage; i++)
                {
                    pm.Add(new PageModel
                    {
                        Active = i == page,
                        Label = i.ToString(),
                        PageNumber = i
                    });
                }

                if(pages.CurrentPage < pages.TotalPages)
                {
                    pm.Add(new PageModel
                    {
                        Active = false,
                        Label = ">",
                        PageNumber = pages.CurrentPage+1
                    });
                    pm.Add(new PageModel
                    {
                        Active = false,
                        Label = ">>",
                        PageNumber = pages.TotalPages
                    });
                }

                return pm;
            }
        }
    }
}
