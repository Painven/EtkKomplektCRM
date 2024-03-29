using EtkBlazorApp.DataAccess;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EtkBlazorApp.Services
{
    [Obsolete(message: "Небезопасная версия с использованием MD5 заменена на криптостойкую")]
    public class MyCustomAuthProvider : AuthenticationStateProvider
    {
        private readonly IUserService auth;
        private readonly IUserInfoChecker userInfoChecker;
        private readonly IJSRuntime js;
        private readonly ProtectedLocalStorage storage;

        public MyCustomAuthProvider(
            IUserService auth,
            IUserInfoChecker userInfoChecker,
            IJSRuntime js,
            ProtectedLocalStorage storage)
        {
            this.storage = storage;
            this.auth = auth;
            this.userInfoChecker = userInfoChecker;
            this.js = js;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var login = await storage.GetAsync<string>("user_login");
                var password = await storage.GetAsync<string>("user_password");

                if (login.Success && password.Success)
                {
                    var state = await AuthenticateUser(new AppUser()
                    {
                        Login = login.Value,
                        Password = password.Value
                    });

                    return state;
                }
            }
            catch
            {

            }

            return GetDefaultState();
        }

        public async Task<AuthenticationState> AuthenticateUser(AppUser userData)
        {
            await userInfoChecker.FillUserInfo(userData);

            string permission = null;//await auth.GetUserPermission(userData.Login, userData.Password);

            throw new NotImplementedException();
            /*
            if (string.IsNullOrWhiteSpace(permission))
            {
                await storage.DeleteAsync("user_login");
                await storage.DeleteAsync("user_password");

                return GetDefaultState();
            }

            var userInfo = (await auth.GetUsers()).Single(u => u.login == userData.Login);
       
            if (!string.IsNullOrWhiteSpace(userInfo.ip) && userInfo.ip != userData.UserIP)
            {
                return GetDefaultState();
            }

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, userData.Login),
                new Claim(ClaimTypes.Role, permission)
            }, "login_form");
            var user = new ClaimsPrincipal(identity);
            var state = new AuthenticationState(user);

            NotifyAuthenticationStateChanged(Task.FromResult(state));

            await auth.UpdateUserLastLoginDate(userData.Login);

            SetUserCookie(userData.Login);

            return state;
            */
        }

        private void SetUserCookie(string name)
        {
            //cookie для того, если user активен в ЛК то отображать склады на странице товара
            string cookieString = $"lk_user={name}; expires=9999-12-31T23:59:59.000Z;path=/;domain=etk-komplekt.ru";
            js.InvokeVoidAsync("CookieFunction.acceptMessage", cookieString);
        }

        public async Task LogOutUser()
        {
            try
            {
                await storage.DeleteAsync("user_login");
                await storage.DeleteAsync("user_password");
            }
            catch
            {

            }
            NotifyAuthenticationStateChanged(Task.FromResult(GetDefaultState()));
        }

        private AuthenticationState GetDefaultState()
        {
            var identity = new ClaimsIdentity(new[]
{
                new Claim(ClaimTypes.Name, "Гость")
            }, authenticationType: string.Empty);
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
            var state = new AuthenticationState(user);
            return state;
        }
    }
}
