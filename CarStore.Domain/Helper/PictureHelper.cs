using Microsoft.AspNetCore.Http;

namespace CarStore.Domain.Helper
{
    public class PictureHelper
    {

        public const string allowedExtensions = ".png, .jpg, .jpeg";
        public const long allowedSize = 12582912;


        public static bool IsAllowedExtension(IFormFile file)
        {
            return allowedExtensions.Contains(Path.GetExtension((file.FileName).ToLower()));
        }

        public static bool IsAllowedSize(IFormFile file)
        {
            return file.Length <= allowedSize;
        }
        //public async static Task<MemoryStream> Upload(IFormFile data)
        //{

        //    using var dataStream = new MemoryStream();
        //    await data.CopyToAsync(dataStream);

        //    return dataStream;
        //}
    }
}
