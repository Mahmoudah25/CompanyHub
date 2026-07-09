using CompanyHub.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyHub.Infrastructure.Storage
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor httpContextAccessor;
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".pdf", ".webp" };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024;
        public LocalFileStorageService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
        }
        public Task<bool> DeleteAsync(string filePath)
        {
            var fileName = Path.GetFileName(new Uri(filePath).LocalPath);
            var folder = new Uri(filePath).LocalPath.Split('/')[2]; // uploads/{folder}/{fileName}
            var fullPath = Path.Combine(env.WebRootPath ?? "wwwroot", "uploads", folder, fileName);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public async Task<(byte[] Content, string ContentType)?> GetAsync(string filePath)
        {
            var fileName = Path.GetFileName(new Uri(filePath).LocalPath);
            var folder = new Uri(filePath).LocalPath.Split('/')[2];
            var fullPath = Path.Combine(env.WebRootPath ?? "wwwroot", "uploads", folder, fileName);

            if (!File.Exists(fullPath))
                return null;

            var bytes = await File.ReadAllBytesAsync(fullPath);
            var contentType = GetContentType(fullPath);
            return (bytes, contentType);
        }

        public async Task<string> UploadAsync(IFormFile file, string folder)
        {
            ValidateFile(file);

            var uploadsRoot = Path.Combine(env.WebRootPath ?? "wwwroot", "uploads", folder);
            Directory.CreateDirectory(uploadsRoot);

            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(uploadsRoot, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var request = httpContextAccessor.HttpContext?.Request;
            var baseUrl = $"{request?.Scheme}://{request?.Host}";
            return $"{baseUrl}/uploads/{folder}/{fileName}";
        }

        private void ValidateFile(IFormFile file)
        {
            if (file.Length == 0)
                throw new ArgumentException("File is empty.");

            if (file.Length > MaxFileSizeBytes)
                throw new ArgumentException("File size exceeds 5 MB limit.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                throw new ArgumentException($"File type '{extension}' is not allowed.");
        }

        private string GetContentType(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream"
            };
        }
    }
}
