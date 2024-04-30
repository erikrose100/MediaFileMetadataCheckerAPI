using MediaFileMetadataCheckerAPI.Models;
using MediaFileMetadataCheckerAPI.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;
using FFMpegCore;
using FFMpegCore.Enums;
using System.Configuration;
using System.Linq;
using MetadataAppConfig;

namespace MediaFileMetadataCheckerAPI.Controllers
{
    /// <summary>
    /// controller for upload large file
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly ILogger<FileUploadController> _logger;

        public FileUploadController(ILogger<FileUploadController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Action for upload large file
        /// </summary>
        /// <remarks>
        /// Request to this action will not trigger any model binding or model validation,
        /// because this is a no-argument action
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        [Route(nameof(UploadLargeFileForMetadata))]
        [ProducesResponseType(typeof(FileStreamResult), 200, "application/json")]
        // [Produces("application/json", "application/json")]
        [DisableFormValueModelBinding]
        public async Task<IActionResult> UploadLargeFileForMetadata()
        {
            var request = HttpContext.Request;

            // validation of Content-Type
            // 1. first, it must be a form-data request
            // 2. a boundary should be found in the Content-Type
            if (!request.HasFormContentType ||
                !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
                string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
            {
                return new UnsupportedMediaTypeResult();
            }

            var boundary = HeaderUtilities.RemoveQuotes(mediaTypeHeader.Boundary.Value).Value!;
            var reader = new MultipartReader(boundary, request.Body);
            var section = await reader.ReadNextSectionAsync();

            // This sample try to get the first file from request and save it
            // Make changes according to your needs in actual use
            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition,
                    out var contentDisposition);

                #pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (hasContentDispositionHeader && contentDisposition.DispositionType.Equals("form-data") &&
                    !string.IsNullOrEmpty(contentDisposition.FileName.Value))
                {
                    // Don't trust any file name, file extension, and file data from the request unless you trust them completely
                    // Otherwise, it is very likely to cause problems such as virus uploading, disk filling, etc
                    // In short, it is necessary to restrict and verify the upload
                    // Here, we just use the temporary folder and a random file name

                    // Get the temporary folder, and combine a random file name with it
                    var fileName = Path.GetRandomFileName();
                    var saveToPath = Path.Combine(Path.GetTempPath(), fileName);

                    using (var targetStream = System.IO.File.Create(saveToPath))
                    {
                        await section.Body.CopyToAsync(targetStream);
                    }

                    IMediaAnalysis mediaInfo = await FFProbe.AnalyseAsync(saveToPath);

                    if (mediaInfo is not null)
                    {
                        HashSet<string> returnProperties = Settings.ReturnProperties.Split(";").ToHashSet();

                        var File = new FileUploadItem();
                        File.Duration = returnProperties.Contains("Duration") ? mediaInfo.Duration : null;
                        File.BitRate =  returnProperties.Contains("BitRate") ? mediaInfo.Format.BitRate : null;
                        File.Format = returnProperties.Contains("Format") ? mediaInfo.Format.FormatLongName : null;
                        File.AudioStreamCount = returnProperties.Contains("AudioStreamCount") ? mediaInfo.AudioStreams.Count : null;
                        File.HashCode = returnProperties.Contains("HashCode") ? mediaInfo.Format.GetHashCode() : null;

                        return Ok(File);
                    }
                    else
                    {
                        return BadRequest("Processing error: could not get file metadata");
                    }

                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                section = await reader.ReadNextSectionAsync();
            }

            // If the code runs to this location, it means that no files have been saved
            return BadRequest("No files data in the request.");
        }
    }
}