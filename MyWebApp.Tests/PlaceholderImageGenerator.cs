using Microsoft.AspNetCore.Http;
using MyWebApp.Data;
using MyWebApp.Data.Interfaces;

namespace MyWebApp.Tests
{
    public class PlaceholderImageGenerator
    {
        private readonly IRandomGenerator _randomGenerator;

        public PlaceholderImageGenerator()
        {
            _randomGenerator = new RandomGenerator();
        }

        public IFormFile GetImage()
        {
            var content = _randomGenerator.GetRandomString(100);
            var fileName = _randomGenerator.GetRandomString(10) + ".jpg";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            return new FormFile(stream, 0, stream.Length, _randomGenerator.GetRandomString(10), fileName);
        }
    }
}
