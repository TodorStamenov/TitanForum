namespace TitaniumForum.Web.Infrastructure.Helpers
{
    using System;

    public class ControllerHelpers
    {
        public static int GetTotalPages(int totalEntries, int entriesPerPage)
        {
            return (int)Math.Ceiling(totalEntries / (double)entriesPerPage);
        }
    }
}