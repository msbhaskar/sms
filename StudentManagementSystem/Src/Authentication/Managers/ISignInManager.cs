namespace StudentManagementSystem.Authentication.Managers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNet.Identity.Owin;

    public interface ISignInManager<T> : IDisposable where T: class
    {
        Task SignInAsync(T user, bool isPersistent, bool rememberBrowser);

        Task<bool> SendTwoFactorCodeAsync(string selectedProvider);

        Task<SignInStatus> ExternalSignInAsync(ExternalLoginInfo loginInfo, bool isPersistent);

        Task<string> GetVerifiedUserIdAsync();

        Task<SignInStatus> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberBrowser);

        Task<bool> HasBeenVerifiedAsync();

        Task<SignInStatus> PasswordSignInAsync(string email, string password, bool rememberMe, bool shouldLockout);
    }
}
