using MyWebApp.TableModels;

namespace MyWebApp.Data
{
    public sealed class PicturesLoader : IPicturesLoader
    {
        private const string _imagesFolderName = "note-images";
        private const string _demoNoteImagesFolderName = "demo-note-images";
        private const string _demoProfileImagesFolderName = "demo-profile-images";
        private const string _defaultImageFolderName = "default";

        private static string GetNewFileName(string filePath)
        {
            var name = RandomGenerator.GetRandomString(80);
            var extension = Path.GetExtension(filePath);
            return $"{name}{extension}";
        }

        private static string GetNewFileName(IFormFile file)
        {
            return GetNewFileName(file.FileName);
        }

        private static async Task SaveFile(IFormFile file, string destinationPath)
        {
            using var stream = new FileStream(destinationPath, FileMode.Create);
            await file.CopyToAsync(stream);
        }

        private static void SaveFile(string filePath, string destinationPath)
        {
            File.Copy(filePath, destinationPath, true);
        }

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

            var defaultImages = LoadDefaultImages();
            DefaultNoteImage = defaultImages.Item1;
            DefaultProfileImage = defaultImages.Item2;
        }

        public NoteImageModel DefaultNoteImage { get; }
        public UserImageModel DefaultProfileImage { get; }

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
                UserId = user.UserId,
                ImageFileName = fileName,
            };

            return model;
        }

        public List<NoteImageModel> LoadDemoNoteImages(List<NoteModel> notes)
        {
            EnsureFolderIsCreated();

            var images = Directory.GetFiles(_demoNoteImagesPath);
            var result = new List<NoteImageModel>();
            var i = 0;
            foreach (var image in images)
            {
                var fileName = GetNewFileName(image);
                SaveFile(image, Path.Combine(_imagesPath, fileName));
                var model = new NoteImageModel()
                {
                    ImageId = RandomGenerator.GetRandomId(),
                    NoteId = notes[i % notes.Count].NoteId,
                    ImageFileName = fileName,
                };
                result.Add(model);
                i += 1;
            }

            return result;
        }

        public List<UserImageModel> LoadDemoProfileImages(List<UserModel> users)
        {
            EnsureFolderIsCreated();

            var images = Directory.GetFiles(_demoProfileImagesPath);
            var result = new List<UserImageModel>();
            var i = 0;
            foreach (var image in images)
            {
                var fileName = GetNewFileName(image);
                SaveFile(image, Path.Combine(_imagesPath, fileName));
                var model = new UserImageModel()
                {
                    ImageId = RandomGenerator.GetRandomId(),
                    UserId = users[i % users.Count].UserId,
                    ImageFileName = fileName,
                };
                result.Add(model);
                i += 1;
            }

            return result;
        }

        private (NoteImageModel, UserImageModel) LoadDefaultImages()
        {
            EnsureFolderIsCreated();

            SaveFile(_defaultImagePath, Path.Combine(_imagesPath, "default.jpg"));
            var defaultNoteImage = new NoteImageModel()
            {
                ImageId = "default",
                NoteId = "default",
                ImageFileName = "default.jpg"
            };
            var defaultProfileImage = new UserImageModel()
            {
                ImageId = "default",
                UserId = "default",
                ImageFileName = "default.jpg"
            };

            return (defaultNoteImage, defaultProfileImage);
        }
    }
}
