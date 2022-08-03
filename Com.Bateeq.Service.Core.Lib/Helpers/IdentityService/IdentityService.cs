namespace Com.DanLiris.Service.Core.Lib.Helpers.IdentityService
{
    public class IdentityService : IIdentityService
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public int TimezoneOffset { get; set; }
    }
}
