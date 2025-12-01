using System.ComponentModel.DataAnnotations;

namespace MarketplaceSystem.Common.Models.Pagination
{
    public abstract class PaginatedRequest
    {
        public int _pageNumber = 1;
        public int _pageSize = 12;
        public virtual int PageNumber
        {
            get => _pageNumber;
            set
            {
                if (value >= 1)
                    _pageNumber = value;
            }
        }
        public virtual int PageSize
        {
            get => _pageSize;
            set
            {
                if (value >= 1 && value <= 100)
                    _pageSize = value;
            }
        }
        [MaxLength(50, ErrorMessage = "Độ dài không được vượt quá 50 ký tự")]
        public virtual string? Search { get; set; }
    }
}
