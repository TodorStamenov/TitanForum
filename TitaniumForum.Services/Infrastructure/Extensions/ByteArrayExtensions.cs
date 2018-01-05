namespace TitaniumForum.Services.Infrastructure.Extensions
{
    using System;

    public static class ByteArrayExtensions
    {
        public static string ConvertImage(this byte[] image)
        {
            return ServiceConstants.DataImage + Convert.ToBase64String(image);
        }
    }
}