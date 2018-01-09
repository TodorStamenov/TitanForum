using System;

namespace TitaniumForum.Web.Infrastructure.Helpers
{
    public class BasePageViewModel
    {
        private const int BackOffset = 5;
        private const int ForwardOffset = 5;

        public int CurrentPage { get; set; }

        public int TotalEntries { get; set; }

        public int EntriesPerPage { get; set; }

        public int TotalPages
        {
            get
            {
                return (int)Math.Ceiling(this.TotalEntries / (double)this.EntriesPerPage);
            }
        }

        public int FirstPage
        {
            get
            {
                return this.CurrentPage - BackOffset <= 1 ? 1 : this.CurrentPage - BackOffset;
            }
        }

        public int LastPage
        {
            get
            {
                return this.CurrentPage + ForwardOffset >= this.TotalPages ? this.TotalPages : this.CurrentPage + ForwardOffset;
            }
        }

        public int PrevPage
        {
            get
            {
                return this.CurrentPage <= 1 ? 1 : this.CurrentPage - 1;
            }
        }

        public int NextPage
        {
            get
            {
                return this.CurrentPage >= this.TotalPages ? this.TotalPages : this.CurrentPage + 1;
            }
        }
    }
}