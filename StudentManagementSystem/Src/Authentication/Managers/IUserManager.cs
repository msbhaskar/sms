namespace StudentManagementSystem.Authentication.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNet.Identity;

    public interface IUserManager<T> : IDisposable where T : class
    {
        Task<IdentityResult> AddLoginAsync(string id, UserLoginInfo login);

        Task<IdentityResult> CreateAsync(T user);

        Task<IList<string>> GetValidTwoFactorProvidersAsync(string userId);

        Task<IdentityResult> ResetPasswordAsync(string id, string code, string password);

        Task<T> FindByNameAsync(string email);

        Task<bool> IsEmailConfirmedAsync(string id);

        Task<IdentityResult> ConfirmEmailAsync(string userId, string code);

        Task<IdentityResult> CreateAsync(T user, string password);

        Task<string> GeneratePasswordResetTokenAsync(string id);

        Task SendEmailAsync(string id, string resetPassword, string s);
    }
}