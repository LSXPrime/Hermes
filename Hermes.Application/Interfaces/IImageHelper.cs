namespace Hermes.Application.Interfaces;

/// <summary>
/// Defines the interface for an image helper.
/// </summary>
public interface IImageHelper
{
    /// <summary>
    /// Processes and uploads an image file to cloud storage.
    /// </summary>
    /// <param name="imageFile">The image file to be processed and uploaded.</param>
    /// <param name="fileName">The name of the image file.</param>
    /// <param name="folderName">The name of the folder in cloud storage to upload the image to.</param>
    /// <returns>
    /// The URL of the uploaded image.
    /// </returns>
    Task<string> ProcessAndUploadImageAsync(Stream imageFile, string fileName, string folderName);
}