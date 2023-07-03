namespace CarrierPortal.Services.PhotoServices
{
    public interface IPhotoListService
    {
        List<string> SavePhotos(List<IFormFile> photoFiles, string folderName);
    }
}