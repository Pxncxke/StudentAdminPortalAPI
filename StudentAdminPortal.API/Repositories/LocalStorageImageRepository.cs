namespace StudentAdminPortal.API.Repositories
{
    public class LocalStorageImageRepository : IImageRepository
    {
        public async Task<string> Upload(IFormFile file, string fileName)
        {
            var filepath = Path.Combine(Directory.GetCurrentDirectory(), @"Resourses\Images\", fileName);

            using Stream stream = new FileStream(filepath, FileMode.Create);

            await file.CopyToAsync(stream);

            return GetServerPath(filepath);
        }

        private string GetServerPath(string fileName)
        {
            return Path.Combine(@"Resourses\Images\", fileName);
        }
    }
}
