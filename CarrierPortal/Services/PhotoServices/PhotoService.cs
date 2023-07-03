namespace CarrierPortal.Services.PhotoServices
{
    public class PhotoService : IPhotoService
    {
        private readonly IWebHostEnvironment _environment;

        public PhotoService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public string SavePhoto(IFormFile photoFile, string folderName)
        {
            // Generate a unique file name
            string uniqueFileName = $"{Guid.NewGuid().ToString()}_{photoFile.FileName}";

            // Determine the upload directory path
            string uploadsDirectory = Path.Combine(_environment.WebRootPath, folderName);

            // Combine the directory path and file name
            string filePath = Path.Combine(uploadsDirectory, uniqueFileName);

            // Save the file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                photoFile.CopyTo(fileStream);
            }

            // Return the file name
            return uniqueFileName;
        }


    }
}
