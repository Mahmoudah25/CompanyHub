using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Application.Common.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadAsync(IFormFile file, string folder); // بيرجع الـ URL/Path
        Task<bool> DeleteAsync(string filePath);
        Task<(byte[] Content, string ContentType)?> GetAsync(string filePath);
    }
}
