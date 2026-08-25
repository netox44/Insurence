using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace Insurence.Helper
{
    public static class FileHelper
    {
        public static string? SaveFile(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                return null;

            folder = folder.Replace("\\", "/").Trim('/');
            var fullFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folder);

            if (!Directory.Exists(fullFolderPath))
                Directory.CreateDirectory(fullFolderPath);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(fullFolderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return $"{folder}/{fileName}".Replace("\\", "/");
        }

        public static void DeleteFile(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            filePath = filePath.TrimStart('/').Replace("\\", "/");
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath);

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}
