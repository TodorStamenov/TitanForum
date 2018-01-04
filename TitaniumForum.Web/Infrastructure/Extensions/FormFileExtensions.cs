namespace TitaniumForum.Web.Infrastructure.Extensions
{
    using System.IO;
    using System.Web;

    public static class FormFileExtensions
    {
        public static byte[] ToByteArray(this HttpPostedFileBase formFile)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                formFile.InputStream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}