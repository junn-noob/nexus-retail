using NexusRetail.Shared.DTOs.SanPham;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusRetail.Shared.DTOs.Common
{
    public class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = [];

        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}
