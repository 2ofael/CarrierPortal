namespace CarrierPortal.Services.PhotoServices
{
    public class PhotoListService : IPhotoListService
    {
        private readonly IWebHostEnvironment _environment;

        public PhotoListService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public List<string> SavePhotos(List<IFormFile> photoFiles, string folderName)
        {
            List<string> savedFileNames = new List<string>();

            foreach (var photoFile in photoFiles)
            {
                // Generate a unique file name
                string uniqueFileName = $"{Guid.NewGuid().ToString()}_{photoFile.FileName}";

                // Determine the upload directory path
                string uploadsDirectory = Path.Combine(_environment.WebRootPath, folderName);

                // Create the directory if it doesn't exist
                if (!Directory.Exists(uploadsDirectory))
                {
                    Directory.CreateDirectory(uploadsDirectory);
                }

                // Combine the directory path and file name
                string filePath = Path.Combine(uploadsDirectory, uniqueFileName);

                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    photoFile.CopyTo(fileStream);
                }

                // Add the file name to the list
                savedFileNames.Add(uniqueFileName);
            }

            return savedFileNames;
        }


    }
}
