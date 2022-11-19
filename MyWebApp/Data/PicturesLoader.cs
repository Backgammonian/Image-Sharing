using MyWebApp.Models;

namespace MyWebApp.Data
{
    public sealed class PicturesLoader
    {
        private const string _imagesFolderName = "images";
        private const string _demoNoteImagesFolderName = "demo-note-images";
        private const string _demoProfileImagesFolderName = "demo-profile-images";
        private const string _defaultImageFolderName = "default-images";

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _wwwRootPath;
        private readonly string _imagesPath;
        private readonly string _demoNoteImagesPath;
        private readonly string _demoProfileImagesPath;
        private readonly string _defaultImagePath;

        public PicturesLoader(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _wwwRootPath = _webHostEnvironment.WebRootPath;
            _imagesPath = $"{_wwwRootPath}/{_imagesFolderName}";
            _demoNoteImagesPath = $"{_wwwRootPath}/{_demoNoteImagesFolderName}";
            _demoProfileImagesPath = $"{_wwwRootPath}/{_demoProfileImagesFolderName}";
            _defaultImagePath = $"{_wwwRootPath}/{_defaultImageFolderName}";
        }

        private string GetNewFileName(string filePath)
        {
            var name = RandomGenerator.GetRandomString(80);
            var extension = Path.GetExtension(filePath);
            return $"{name}{extension}";
        }

        private string GetNewFileName(IFormFile file)
        {
            return GetNewFileName(file.FileName);
        }

        private async Task SaveFile(IFormFile file, string destinationPath)
        {
            using var stream = new FileStream(destinationPath, FileMode.Create);
            await file.CopyToAsync(stream);
        }

        private void SaveFile(string sourcePath, string destinationPath)
        {
            File.Copy(sourcePath, destinationPath, true);
        }

        private void EnsureFolderIsCreated()
        {
            if (!Directory.Exists(_imagesPath))
            {
                Directory.CreateDirectory(_imagesPath);
            }
        }

        public async Task<NoteImageModel> LoadNoteImage(IFormFile image, NoteModel note)
        {
            EnsureFolderIsCreated();

            var fileName = GetNewFileName(image);
            await SaveFile(image, Path.Combine(_imagesPath, fileName));
            var model = new NoteImageModel()
            {
                ImageId = RandomGenerator.GetRandomId(),
                NoteId = note.NoteId,
                ImageFileName = fileName,
                UploadTime = DateTimeOffset.Now
            };

            return model;
        }

        public async Task<UserImageModel> LoadProfileImage(IFormFile image, UserModel user)
        {
            EnsureFolderIsCreated();

            var fileName = GetNewFileName(image);
            await SaveFile(image, Path.Combine(_imagesPath, fileName));
            var model = new UserImageModel()
            {
                ImageId = RandomGenerator.GetRandomId(),
                UserId = user.Id,
                ImageFileName = fileName,
                UploadTime = DateTimeOffset.Now
            };

            return model;
        }

        public void LoadDefaultImage()
        {
            EnsureFolderIsCreated();

            var defaultFileName = "default.jpg";
            SaveFile(Path.Combine(_defaultImagePath, defaultFileName),
                Path.Combine(_imagesPath, defaultFileName));
        }

        public UserImageModel GetDefaultProfileImage()
        {
            return new UserImageModel()
            {
                ImageId = "default",
                UserId = "default",
                ImageFileName = "default.jpg",
                UploadTime = DateTimeOffset.MinValue
            };
        }

        public NoteImageModel GetDefaultNoteImage()
        {
            return new NoteImageModel()
            {
                ImageId = "default",
                NoteId = "default",
                ImageFileName = "default.jpg"
            };
        }

        public List<NoteImageModel> LoadDemoNoteImages(NoteModel[] notes)
        {
            EnsureFolderIsCreated();

            var images = Directory.GetFiles(_demoNoteImagesPath);
            var result = new List<NoteImageModel>();
            var i = 0;
            foreach (var image in images)
            {
                var fileName = Path.GetFileName(image);
                SaveFile(image, Path.Combine(_imagesPath, fileName));
                var currentModelIndex = i % notes.Length;
                var model = new NoteImageModel()
                {
                    ImageId = RandomGenerator.GetRandomId(),
                    NoteId = notes[currentModelIndex].NoteId,
                    ImageFileName = fileName,
                    UploadTime = DateTimeOffset.Now
                };
                result.Add(model);
                i += 1;
            }

            return result;
        }

        public List<UserImageModel> LoadDemoProfileImages(UserModel[] users)
        {
            EnsureFolderIsCreated();

            var images = Directory.GetFiles(_demoProfileImagesPath);
            var result = new List<UserImageModel>();
            var i = 0;
            foreach (var image in images)
            {
                var fileName = Path.GetFileName(image);
                SaveFile(image, Path.Combine(_imagesPath, fileName));
                var currentModelIndex = i % users.Length;
                var model = new UserImageModel()
                {
                    ImageId = RandomGenerator.GetRandomId(),
                    UserId = users[currentModelIndex].Id,
                    ImageFileName = fileName,
                    UploadTime = DateTimeOffset.Now
                };
                result.Add(model);

                i += 1;
            }

            return result;
        }
    }
}
