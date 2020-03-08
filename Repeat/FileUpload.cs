using Microsoft.AspNetCore.Http;
using System;
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
        public static string ToString(byte[] data)
        {
            var base64 = Convert.ToBase64String(data);
            var imgSrc = String.Format("data:image/gif;base64,{0}", base64);
            return imgSrc;
        }
    }
}
