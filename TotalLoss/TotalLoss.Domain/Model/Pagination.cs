using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalLoss.Domain.Model
{
    public class Pagination<T>
    {
        private int _maxPageSize = 100;
        private int _pageSize = 10;
        private int _totalPage = 0;

        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value < _maxPageSize) ? value : _maxPageSize;
            }
        }

        public int PageNumber { get; set; }

        public int RowsCount { get; set; }

        public int TotalPage
        {
            get
            {
                return _totalPage;
            }
            set
            {
                //informa a quantidade de registros
                this.RowsCount = value;

                //Calcula o total de páginas
                _totalPage = value > 0 ? ((value / _pageSize) + 1) : 0;
            }
        }

        public IList<T> Page { get; set; }
    }
}
