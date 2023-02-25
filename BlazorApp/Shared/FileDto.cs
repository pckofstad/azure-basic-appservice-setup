using BlazorApp.Server.DbContext.Models;

namespace BlazorApp.Shared
{
    public class FileDto
    {
        public DateTimeOffset Created { get; set; }
        public string FileName { get; set; }

        public FileDto() { }

        public FileDto(string fileName, DateTimeOffset fileCreated)
        {
            FileName = fileName;
            Created = fileCreated;

        }
    }
}
