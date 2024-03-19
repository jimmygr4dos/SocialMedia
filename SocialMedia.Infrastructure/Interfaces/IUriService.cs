using SocialMedia.Core.QueryFilters;
using System;

namespace SocialMedia.Infrastructure.Interfaces
{
    public interface IUriService
    {
        Uri GetPostPaginationUri(PostQueryFilter filters, string actionUrl);
    }
}
