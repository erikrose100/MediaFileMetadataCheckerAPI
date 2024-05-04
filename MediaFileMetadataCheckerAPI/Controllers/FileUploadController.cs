using MediaFileMetadataCheckerAPI.Models;
using MediaFileMetadataCheckerAPI.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;
using System.Configuration;
using FFMpegCore;
using FFMpegCore.Enums;
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
         private readonly Settings _settings;
        private readonly ILogger<FileUploadController> _logger;

        public FileUploadController(ILogger<FileUploadController> logger, IOptionsSnapshot<Settings> settings)
        {
            _logger = logger;
            _settings = settings.Value;
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
        [DisableFormValueModelBinding]
        public async Task<IActionResult> UploadLargeFileForMetadata()
        {
            var request = HttpContext.Request;

            // Validation of Content-Type
            // 1. First, it must be a form-data request
            // 2. A boundary should be found in the Content-Type
            if (!request.HasFormContentType ||
                !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
                string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
            {
                return new UnsupportedMediaTypeResult();
            }

            var boundary = HeaderUtilities.RemoveQuotes(mediaTypeHeader.Boundary.Value).Value!;
            var reader = new MultipartReader(boundary, request.Body);
            var section = await reader.ReadNextSectionAsync();

            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition,
                    out var contentDisposition);

                #pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (hasContentDispositionHeader && contentDisposition.DispositionType.Equals("form-data") &&
                    !string.IsNullOrEmpty(contentDisposition.FileName.Value))
                {
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
                        // Get return properties from App Config and only return configured properties
                        HashSet<string> returnProperties = _settings.ReturnProperties.Split(";").ToHashSet();

                        var File = new FileUploadItem {
                            Duration = returnProperties.Contains("Duration") ? mediaInfo.Duration : null,
                            BitRate =  returnProperties.Contains("BitRate") ? mediaInfo.Format.BitRate : null,
                            Format = returnProperties.Contains("Format") ? mediaInfo.Format.FormatLongName : null,
                            AudioStreamCount = returnProperties.Contains("AudioStreamCount") ? mediaInfo.AudioStreams.Count : null,
                            HashCode = returnProperties.Contains("HashCode") ? mediaInfo.Format.GetHashCode() : null
                        };

                        return Ok(File);
                    }
                    else
                    {
                        return BadRequest("Processing error: could not get file metadata");
                    }

                }
                section = await reader.ReadNextSectionAsync();
            }

            return BadRequest("No files data in the request.");
        }
    }
}