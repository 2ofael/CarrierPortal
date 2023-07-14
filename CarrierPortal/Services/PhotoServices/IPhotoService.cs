namespace CarrierPortal.Services.PhotoServices
{
    public interface IPhotoService
    {
        string SavePhoto(IFormFile photoFile, string folderName, bool isFilePath = false);
    }
}