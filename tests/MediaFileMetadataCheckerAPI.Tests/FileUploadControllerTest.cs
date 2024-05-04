using MediaFileMetadataCheckerAP.Models;
using MediaFileMetadataCheckerAPI.Models;
using Moq;

namespace MediaFileMetadataCheckerAPI.Tests;

public class FileUploadControllerTest
{
    [Fact]
    public async Task FiledUploadTest()
    {
        var fileUploadItem = new FileUploadItem {
            Duration = new TimeSpan(1, 0, 0),
            Format = "Test Format",
            BitRate = 0,
            HashCode = 40,
            AudioStreamCount = 1,
        };

    }
}