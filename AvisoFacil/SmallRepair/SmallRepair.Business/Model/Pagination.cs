using System;
using System.Collections.Generic;
using System.Text;

namespace SmallRepair.Business.Model
{
    public class Pagination<TEntity>
    {
        private int _maximumpagesize = 100;
        private int _pageSize = 10;
        private int _totalPages = 0;

        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value < _maximumpagesize) ? value : _maximumpagesize;
            }
        }

        public int PageNumber { get; set; }

        public int TotalRecords { get; set; }

        public int TotalPages
        {
            get
            {
                return _totalPages;
            }
            set
            {
                //Calcula o total de páginas
                _totalPages = value > 0 ? ((value / _pageSize) + 1) : 0;
            }
        }

        public IList<TEntity> Page { get; set; }
    }
}
