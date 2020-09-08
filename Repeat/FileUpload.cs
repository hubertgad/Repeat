using Microsoft.AspNetCore.Http;
using Repeat.Domain.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace Repeat
{
    public class FileUpload
    {
        [Display(Name = "File")]
        public IFormFile FormFile { get; set; }

        public static byte[] ToByteArray(FileUpload FileUpload)
        {
            if (FileUpload.FormFile != null && FileUpload.FormFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    FileUpload.FormFile.CopyToAsync(memoryStream);
                    if (memoryStream.Length < 2097152)
                    {
                        return memoryStream.ToArray();
                    }
                }
            }
            return null;
        }
        public static string ToString(byte[] data)
        {
            var base64 = Convert.ToBase64String(data);
            var imgSrc = String.Format("data:image/gif;base64,{0}", base64);
            return imgSrc;
        }

        public async Task<Question> UpdatePictureStateAsync(Question question)
        {
            if (this.FormFile != null && this.FormFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await this.FormFile.CopyToAsync(memoryStream);
                if (memoryStream.Length < 2097152)
                {
                    question.Picture ??= new Picture();
                    question.Picture.Data = memoryStream.ToArray();
                }
                else
                {
                    return null;
                }
            }
            return question;
        }
    }
}
