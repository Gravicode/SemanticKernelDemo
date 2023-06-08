using SemanticKernelDemo.Data;

namespace CustomSemanticKernel;
public interface IImageCustom
{
    /// <summary>
    /// Generate an image matching the given description
    /// </summary>
    /// <param name="description">Image description</param>
    /// <param name="width">Image width in pixels</param>
    /// <param name="height">Image height in pixels</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Generated image in base64 format or image URL</returns>
    public Task<List<ImageModel>> GenerateImageAsync(
        string Prompt,
        int NumImages,
        string ImageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ImageData"></param>
    /// <param name="ImageFileName"></param>
    /// <param name="NumberImages"></param>
    /// <param name="ImageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<List<ImageModel>> GenerateImageVariationsAsync(
       byte[] ImageData,
       string ImageFileName,
       int NumberImages,
       string ImageSize,
       CancellationToken cancellationToken = default);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ImageData"></param>
    /// <param name="ImageFileName"></param>
    /// <param name="Prompt"></param>
    /// <param name="MaskData"></param>
    /// <param name="MaskFileName"></param>
    /// <param name="NumberImages"></param>
    /// <param name="ImageSize"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<List<ImageModel>> GenerateImageEditAsync(
       byte[] ImageData,
       string ImageFileName,
       string Prompt,
       byte[] MaskData,
       string MaskFileName,
       int NumberImages,
       string ImageSize,
       CancellationToken cancellationToken = default);
}
/*
 Prompt = InputPrompt,
                    N = NumImages,
                    Size = ImageSize,
                    ResponseFormat = StaticValues.ImageStatics.ResponseFormat.Url,
                    User = Username
 */
/*
  Image = SelectedFile.Content,
                    ImageName = SelectedFile.FileName,
                    N = NumImages,
                    Size = ImageSize,
                    ResponseFormat = StaticValues.ImageStatics.ResponseFormat.Url,
                    User = Username
 */
/*
  Image = SelectedFile.Content,
                    ImageName = SelectedFile.FileName,
                    Prompt = InputPrompt,
                    Mask = SelectedMask.Content,
                    MaskName = SelectedMask.FileName,
                    N = NumImages,
                    Size = ImageSize,
                    ResponseFormat = StaticValues.ImageStatics.ResponseFormat.Url,
                    User = Username
 */ 