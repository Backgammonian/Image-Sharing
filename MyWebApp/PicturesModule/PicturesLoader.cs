using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Data.Interfaces;
using MyWebApp.Models;
using MyWebApp.PicturesModule.Interfaces;

namespace MyWebApp.PicturesModule
{
    public sealed class PicturesLoader : IPicturesLoader
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPicturesSaver _picturesSaver;
        private readonly IRandomGenerator _randomGenerator;
        private readonly ApplicationDbContext _dbContext;
        private readonly string _wwwRootPath;
        private readonly string _imagesPath;
        private readonly string _demoNoteImagesPath;
        private readonly string _demoProfileImagesPath;
        private readonly string _defaultImagePath;

        public PicturesLoader(IWebHostEnvironment webHostEnvironment,
            IPicturesSaver picturesSaver,
            IRandomGenerator randomGenerator,
            ApplicationDbContext dbContext)
        {
            _webHostEnvironment = webHostEnvironment;
            _picturesSaver = picturesSaver;
            _dbContext = dbContext;
            _randomGenerator = randomGenerator;
            _wwwRootPath = _webHostEnvironment.WebRootPath;
            _imagesPath = $"{_wwwRootPath}/{Constants.ImagesFolderName}";
            _demoNoteImagesPath = $"{_wwwRootPath}/{Constants.DemoNoteImagesFolderName}";
            _demoProfileImagesPath = $"{_wwwRootPath}/{Constants.DemoProfileImagesFolderName}";
            _defaultImagePath = $"{_wwwRootPath}/{Constants.DefaultImageFolderName}";
        }

        public string GetNewFileName(string filePath)
        {
            var name = _randomGenerator.GetRandomString(60);
            var extension = Path.GetExtension(filePath);

            return $"{name}{extension}";
        }

        public string GetNewFileName(IFormFile file)
        {
            return GetNewFileName(file.FileName);
        }

        public void EnsureFolderIsCreated()
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
            await _picturesSaver.SaveFile(image, savePath);

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
            await _picturesSaver.SaveFile(image, savePath);

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

            var sourcePath = Path.Combine(_defaultImagePath, Constants.DefaultImageName);
            var savePath = Path.Combine(_imagesPath, Constants.DefaultImageName);
            _picturesSaver.SaveFile(sourcePath, savePath);
        }

        public UserImageModel GetDefaultProfileImage()
        {
            return new UserImageModel()
            {
                ImageId = Constants.DefaultImageNameWithouExtension,
                UserId = Constants.DefaultImageNameWithouExtension,
                ImageFileName = Constants.DefaultImageName,
                UploadTime = DateTimeOffset.MinValue
            };
        }

        public NoteImageModel GetDefaultNoteImage()
        {
            return new NoteImageModel()
            {
                ImageId = Constants.DefaultImageNameWithouExtension,
                NoteId = Constants.DefaultImageNameWithouExtension,
                ImageFileName = Constants.DefaultImageName,
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
                _picturesSaver.SaveFile(imagePath, savePath);
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
                _picturesSaver.SaveFile(imagePath, savePath);
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
