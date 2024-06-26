using MediaFileMetadataCheckerAPI.Models;
using Moq;

namespace MediaFileMetadataCheckerAPI.Tests;

public class FileUploadModelTest
{
    [Fact]
    public void FiledUploadItemTest()
    {
        var fileUploadItem = new FileUploadItem {
            Duration = new TimeSpan(0, 1, 0),
            Format = "QuickTime / MOV",
            BitRate = 733621,
            HashCode = 20117377,
            AudioStreamCount = 1,
        };

        Assert.NotNull(fileUploadItem);
        Assert.Equal("00:01:00", fileUploadItem.Duration.ToString());
        Assert.Equal("QuickTime / MOV", fileUploadItem.Format);
        Assert.Equal(733621, fileUploadItem.BitRate);
        Assert.Equal(20117377, fileUploadItem.HashCode);
        Assert.Equal(1, fileUploadItem.AudioStreamCount);
    }
}