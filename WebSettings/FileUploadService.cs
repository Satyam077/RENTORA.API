using Microsoft.AspNetCore.Http;

namespace RENTORA.API.WebSettings
{
    public interface IFileUploadService
    {
        Task<string> UploadFileAsync(IFormFile file, string folder = "documents");
        Task<bool> DeleteFileAsync(string fileUrl);
        bool IsValidFileType(IFormFile file, string[] allowedExtensions);
        bool IsValidFileSize(IFormFile file, long maxSizeInMB = 10);
    }

    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileUploadService> _logger;
        private readonly string _uploadsFolder;

        public FileUploadService(IWebHostEnvironment environment, ILogger<FileUploadService> logger)
        {
            _environment = environment;
            _logger = logger;
            _uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

            // Ensure uploads directory exists
            if (!Directory.Exists(_uploadsFolder))
            {
                Directory.CreateDirectory(_uploadsFolder);
            }
        }

        /// <summary>
        /// Uploads a file to the server and returns the file URL
        /// </summary>
        /// <param name="file">The file to upload</param>
        /// <param name="folder">Subfolder within uploads directory (e.g., "documents", "images")</param>
        /// <returns>The relative URL of the uploaded file</returns>
        public async Task<string> UploadFileAsync(IFormFile file, string folder = "documents")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("File is empty or null");
                }

                // Create subfolder if it doesn't exist
                var folderPath = Path.Combine(_uploadsFolder, folder);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Generate unique filename
                var fileName = GenerateUniqueFileName(file.FileName);
                var filePath = Path.Combine(folderPath, fileName);

                // Save file to disk
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return relative URL
                var relativeUrl = $"/uploads/{folder}/{fileName}";
                _logger.LogInformation($"File uploaded successfully: {relativeUrl}");

                return relativeUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                throw new Exception($"Error uploading file: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a file from the server
        /// </summary>
        /// <param name="fileUrl">The relative URL of the file to delete</param>
        /// <returns>True if deleted successfully, false otherwise</returns>
        public async Task<bool> DeleteFileAsync(string fileUrl)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileUrl))
                {
                    return false;
                }

                // Convert URL to physical path
                var fileName = fileUrl.Replace("/uploads/", "").Replace("/", "\\");
                var filePath = Path.Combine(_uploadsFolder, fileName);

                if (File.Exists(filePath))
                {
                    await Task.Run(() => File.Delete(filePath));
                    _logger.LogInformation($"File deleted successfully: {fileUrl}");
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting file: {fileUrl}");
                return false;
            }
        }

        /// <summary>
        /// Validates if the file type is allowed
        /// </summary>
        /// <param name="file">The file to validate</param>
        /// <param name="allowedExtensions">Array of allowed extensions (e.g., [".pdf", ".doc", ".docx"])</param>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValidFileType(IFormFile file, string[] allowedExtensions)
        {
            if (file == null)
            {
                return false;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }

        /// <summary>
        /// Validates if the file size is within the allowed limit
        /// </summary>
        /// <param name="file">The file to validate</param>
        /// <param name="maxSizeInMB">Maximum file size in megabytes</param>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValidFileSize(IFormFile file, long maxSizeInMB = 5)
        {
            if (file == null)
            {
                return false;
            }

            var maxSizeInBytes = maxSizeInMB * 1024 * 1024;
            return file.Length <= maxSizeInBytes;
        }

        /// <summary>
        /// Generates a unique filename to prevent conflicts
        /// </summary>
        /// <param name="originalFileName">The original filename</param>
        /// <returns>A unique filename</returns>
        private string GenerateUniqueFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);
            
            // Remove special characters and spaces
            fileNameWithoutExtension = new string(fileNameWithoutExtension
                .Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_')
                .ToArray());

            // Generate unique name with timestamp
            var uniqueFileName = $"{fileNameWithoutExtension}_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid().ToString("N").Substring(0, 8)}{extension}";
            
            return uniqueFileName;
        }
    }
}
