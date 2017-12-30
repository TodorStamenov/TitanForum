namespace TitaniumForum.Web.Infrastructure.Extensions
{
    using System.Web.Mvc;

    public static class TempDataDictionaryExtensions
    {
        public static void AddSuccessMessage(this TempDataDictionary tempData, string message)
        {
            tempData[WebConstants.TempDataSuccessMessage] = message;
        }

        public static void AddErrorMessage(this TempDataDictionary tempData, string message)
        {
            tempData[WebConstants.TempDataErrorMessage] = message;
        }
    }
}