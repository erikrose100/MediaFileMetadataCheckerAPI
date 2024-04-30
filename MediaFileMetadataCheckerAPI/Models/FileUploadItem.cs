using System.Web;
using System;

namespace MediaFileMetadataCheckerAPI.Models;

public class FileUploadItem
{
    public TimeSpan? Duration { get; set; }
    public string? Format { get; set; }
    public double? BitRate { get; set; }
    public int? HashCode { get; set; }
    public int? AudioStreamCount { get; set; }
}