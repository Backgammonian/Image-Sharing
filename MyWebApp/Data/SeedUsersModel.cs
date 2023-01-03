using MyWebApp.Models;

namespace MyWebApp.Data
{
    public sealed class SeedUsersModel
    {
        public UserModel Admin { get; set; }
        public UserModel[] Users { get; set; }
    }
}
