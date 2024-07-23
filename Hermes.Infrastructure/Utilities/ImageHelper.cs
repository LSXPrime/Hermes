using Hermes.Application.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Hermes.Infrastructure.Utilities;

public class ImageHelper(ICloudStorageHelper cloudStorageHelper) : IImageHelper
{
    /// <summary>
    /// Processes and uploads an image file to cloud storage.
    /// </summary>
    /// <param name="imageFile">The image file to be processed and uploaded.</param>
    /// <param name="fileName">The name of the image file.</param>
    /// <param name="folderName">The name of the folder in cloud storage to upload the image to.</param>
    /// <returns>
    /// The URL of the uploaded image in cloud storage.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if the provided image file is invalid.</exception>
    public async Task<string> ProcessAndUploadImageAsync(Stream imageFile, string fileName, string folderName)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            throw new ArgumentException("Invalid image file.");
        }

        using var image = await Image.LoadAsync(imageFile);

        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Size = new Size(800, 0)
        }));


        using var outputStream = new MemoryStream();
        await image.SaveAsync(outputStream, new JpegEncoder());

        outputStream.Position = 0;
        return await cloudStorageHelper.UploadFileAsync(outputStream,
            fileName,
            folderName);
    }
}