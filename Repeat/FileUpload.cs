using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.IO;
namespace Repeat
{
    public class FileUpload
    {
        [Display(Name = "File")]
        public IFormFile FormFile { get; set; }

        public static byte[] ToByteArray(FileUpload FileUpload)
        {
            using (var memoryStream = new MemoryStream())
            {
                FileUpload.FormFile.CopyToAsync(memoryStream);
                if (memoryStream.Length < 2097152)
                {
                    return memoryStream.ToArray();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
