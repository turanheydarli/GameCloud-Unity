using System;

namespace GameCloud.Models
{
    [Serializable]
    public class PageableRequest
    {
        public int page;
        public int size;
    }

    [Serializable]
    public class PageableListResponse<T>
    {
        public T[] items;
        public int totalPages;
        public int currentPage;
        public int pageSize;
        public int totalItems;
    }
}