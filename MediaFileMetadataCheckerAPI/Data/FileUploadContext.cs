using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MediaFileMetadataCheckerAPI.Models;

    public class FileUploadContext : DbContext
    {
        public FileUploadContext (DbContextOptions<FileUploadContext> options)
            : base(options)
        {
        }

        public DbSet<MediaFileMetadataCheckerAPI.Models.FileUploadItem> FileUploadItem { get; set; } = default!;
    }
