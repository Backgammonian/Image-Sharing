using Microsoft.EntityFrameworkCore;
using MyWebApp.Models;

namespace MyWebApp.Data
{
    public sealed class PicturesLoader
    {
        private const string _imagesFolderName = "images";
        private const string _demoNoteImagesFolderName = "demo-note-images";
        private const string _demoProfileImagesFolderName = "demo-profile-images";
        private const string _defaultImageFolderName = "default-images";
        private const string _defaultImageNameWithouExtension = "default";
        public const string DefaultImageName = "default.jpg";

        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _dbContext;
        private readonly RandomGenerator _randomGenerator;
        private readonly string _wwwRootPath;
        private readonly string _imagesPath;
        private readonly string _demoNoteImagesPath;
        private readonly string _demoProfileImagesPath;
        private readonly string _defaultImagePath;

        public PicturesLoader(IWebHostEnvironment webHostEnvironment,
            ApplicationDbContext dbContext,
            RandomGenerator randomGenerator)
        {
            _webHostEnvironment = webHostEnvironment;
            _dbContext = dbContext;
            _randomGenerator = randomGenerator;
            _wwwRootPath = _webHostEnvironment.WebRootPath;
            _imagesPath = $"{_wwwRootPath}/{_imagesFolderName}";
            _demoNoteImagesPath = $"{_wwwRootPath}/{_demoNoteImagesFolderName}";
            _demoProfileImagesPath = $"{_wwwRootPath}/{_demoProfileImagesFolderName}";
            _defaultImagePath = $"{_wwwRootPath}/{_defaultImageFolderName}";
        }

        private string GetNewFileName(string filePath)
        {
            var name = _randomGenerator.GetRandomString(60);
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
            var savePath = Path.Combine(_imagesPath, fileName);
            await SaveFile(image, savePath);

            return new NoteImageModel()
            {
                ImageId = _randomGenerator.GetRandomId(),
                NoteId = note.NoteId,
                ImageFileName = fileName,
                UploadTime = DateTimeOffset.Now
            };
        }

        public async Task<UserImageModel> LoadProfileImage(IFormFile image, UserModel user)
        {
            EnsureFolderIsCreated();

            var fileName = GetNewFileName(image);
            var savePath = Path.Combine(_imagesPath, fileName);
            await SaveFile(image, savePath);

            return new UserImageModel()
            {
                ImageId = _randomGenerator.GetRandomId(),
                UserId = user.Id,
                ImageFileName = fileName,
                UploadTime = DateTimeOffset.Now
            };
        }

        public void LoadDefaultImage()
        {
            EnsureFolderIsCreated();

            var sourcePath = Path.Combine(_defaultImagePath, DefaultImageName);
            var savePath = Path.Combine(_imagesPath, DefaultImageName);
            SaveFile(sourcePath, savePath);
        }

        public UserImageModel GetDefaultProfileImage()
        {
            return new UserImageModel()
            {
                ImageId = _defaultImageNameWithouExtension,
                UserId = _defaultImageNameWithouExtension,
                ImageFileName = DefaultImageName,
                UploadTime = DateTimeOffset.MinValue
            };
        }

        public NoteImageModel GetDefaultNoteImage()
        {
            return new NoteImageModel()
            {
                ImageId = _defaultImageNameWithouExtension,
                NoteId = _defaultImageNameWithouExtension,
                ImageFileName = DefaultImageName,
                UploadTime = DateTimeOffset.MinValue
            };
        }

        public List<NoteImageModel> LoadDemoNoteImages(NoteModel[] notes)
        {
            EnsureFolderIsCreated();

            var imagesPaths = Directory.GetFiles(_demoNoteImagesPath);
            var images = new List<NoteImageModel>();
            var i = 0;
            foreach (var imagePath in imagesPaths)
            {
                var fileName = Path.GetFileName(imagePath);
                var savePath = Path.Combine(_imagesPath, fileName);
                SaveFile(imagePath, savePath);
                var currentModelIndex = i % notes.Length;

                images.Add(new NoteImageModel()
                {
                    ImageId = _randomGenerator.GetRandomId(),
                    NoteId = notes[currentModelIndex].NoteId,
                    ImageFileName = fileName,
                    UploadTime = DateTimeOffset.Now
                });

                i += 1;
            }

            return images;
        }

        public List<UserImageModel> LoadDemoProfileImages(UserModel[] users)
        {
            EnsureFolderIsCreated();

            var imagesPaths = Directory.GetFiles(_demoProfileImagesPath);
            var images = new List<UserImageModel>();
            var i = 0;
            foreach (var imagePath in imagesPaths)
            {
                var fileName = Path.GetFileName(imagePath);
                var savePath = Path.Combine(_imagesPath, fileName);
                SaveFile(imagePath, savePath);
                var currentModelIndex = i % users.Length;

                images.Add(new UserImageModel()
                {
                    ImageId = _randomGenerator.GetRandomId(),
                    UserId = users[currentModelIndex].Id,
                    ImageFileName = fileName,
                    UploadTime = DateTimeOffset.Now
                });

                i += 1;
            }

            return images;
        }

        public async Task<UserImageModel> GetUserCurrentProfilePicture(UserModel? user)
        {
            if (user == null)
            {
                return GetDefaultProfileImage();
            }

            var profilePicture = await _dbContext.ProfileImages.AsNoTracking().OrderBy(x => x.UploadTime).LastOrDefaultAsync(x => x.UserId == user.Id);
            return profilePicture ?? GetDefaultProfileImage();
        }
    }
}
