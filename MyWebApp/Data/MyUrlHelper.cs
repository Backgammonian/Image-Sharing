namespace MyWebApp.Data
{
    public sealed class MyUrlHelper
    {
        private readonly HttpContext? _currentContext;

        public MyUrlHelper(IHttpContextAccessor contextAccessor)
        {
            _currentContext = contextAccessor.HttpContext;
        }

        public string GetCurrentUrl()
        {
            if (_currentContext != null)
            {
                var request = _currentContext.Request;
                var host = request.Host.ToUriComponent();
                var path = request.Path.ToUriComponent();

                return $"{request.Scheme}://{host}{path}";
            }

            return "123";
        }
    }
}
