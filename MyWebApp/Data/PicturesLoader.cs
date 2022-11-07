using MyWebApp.TableModels;

namespace MyWebApp.Data
{
    public sealed class PicturesLoader
    {
        private const string _imagesFolderName = "images";
        private const string _demoNoteImagesFolderName = "demo-note-images";
        private const string _demoProfileImagesFolderName = "demo-profile-images";
        private const string _defaultImageFolderName = "default";

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

            //NOTE: don't forget to load these
            DefaultProfileImage = new UserImageModel();
            DefaultNoteImage = new NoteImageModel();
        }

        public UserImageModel DefaultProfileImage { get; private set; }
        public NoteImageModel DefaultNoteImage { get; private set; }

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

        private void SaveFile(string filePath, string destinationPath)
        {
            File.Copy(filePath, destinationPath);
        }

        private void EnsureFolderIsCreated()
        {
            if (!Directory.Exists(_imagesPath))
            {
                Directory.CreateDirectory(_imagesPath);
            }
        }

        public (NoteImageModel, UserImageModel) LoadDefaultImages()
        {
            EnsureFolderIsCreated();

            SaveFile(Path.Combine(_defaultImagePath, "default.jpg"), Path.Combine(_imagesPath, "default.jpg"));
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

            DefaultProfileImage = defaultProfileImage;
            DefaultNoteImage = defaultNoteImage;

            return (defaultNoteImage, defaultProfileImage);
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

        //use methods from above???
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
                var currentUserIndex = i % users.Count;
                users[currentUserIndex].CurrentProfilePictureNumber += 1;
                var model = new UserImageModel()
                {
                    ImageId = RandomGenerator.GetRandomId(),
                    UserId = users[currentUserIndex].UserId,
                    ImageFileName = fileName,
                    ProfilePictureNumber = users[currentUserIndex].CurrentProfilePictureNumber
                };
                result.Add(model);

                i += 1;
            }

            return result;
        }
    }
}
