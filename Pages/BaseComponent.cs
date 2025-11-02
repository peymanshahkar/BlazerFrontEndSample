using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Threading.Tasks;
using DarianTablet.Shared.Dto;
using Blazored.LocalStorage;
using System.IdentityModel.Tokens.Jwt;

namespace Client.Pages
{
    public class BaseComponent : ComponentBase
    {


        //internal HubConnection? hubConnection { get; set; }

        [Inject]
        public IJSRuntime js  { get; set; }

        [Inject]
        public ApiClient ApiClient { get; set; }
        public string Token { get; set; }

        [Inject]
        public NavigationManager nv { get; set; }

        [Inject]
        public ProtectedLocalStorage ProtectedLocalStorage { get; set; }

        
        Blazored.LocalStorage.ISyncLocalStorageService localStorage;

        internal UserDto _Profile { get; set; }

        //public Language SelectedLanguage { get; set; }
        //public Language CalendarType { get; set; } = Language.Farsi;


        public int rowNum = 0;

/*
        public async Task SetLanguage()
        {
            try
            {
                SelectedLanguage = (await ProtectedLocalStorage.GetAsync<Language>("SelectedLanguage")).Value;
                CalendarType = (await ProtectedLocalStorage.GetAsync<Language>("CalendarType")).Value;
            }
            catch (Exception ex)
            {
                SelectedLanguage = Language.Farsi;
                CalendarType = Language.Farsi;
            }
        }
*/
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;


           // await SetLanguage();


        }

        public async Task SetToken()
        {
            try
            {

                var result = await ProtectedLocalStorage.GetAsync<string>("Token");

                // بررسی اینکه آیا توکن موجود است  
                if (result.Success)
                {
                    Token = result.Value;
                }
                else
                {
                    OpenLink("/Login");
                    return; // اینجا از خروجی و ادامه جلوگیری می‌کنیم اگر توکن وجود ندارد  
                }
            }
            catch (Exception ex)
            {
                // در صورت بروز خطا، کاربر را به صفحه ورود هدایت کنید  
                OpenLink("/Login");
                return; // همین‌جا متوقف می‌شویم  
            }

            // بررسی اینکه آیا توکن خالی است یا خیر  
            if (string.IsNullOrEmpty(Token))
            {
                OpenLink("/Login");
                return;
            }

            if(IsTokenExpired(Token))
            {
                OpenLink("/Login");
            }

            // تنظیم توکن در ApiClient  
            ApiClient.SetToken(Token);




            // تلاش برای دریافت اطلاعات کاربر  
            try
            {
                _Profile = await ApiClient.GetAsync<UserDto>("Auth/GetUserInfo");
            }
            catch (Exception ex)
            {
                // به صورت اختیاری می‌توانید در اینجا خطا را مدیریت کنید  
                // مثل نشان دادن پیام خطا یا هدایت مجدد به صفحه ورود  
                OpenLink("/Login");
            }
        }

        //internal bool IsInRole(GeneralRoles role)
        //{
        //    return (_Profile.Roles.FirstOrDefault(x => x == role.ToString()) != null);
        //}
        public bool IsTokenExpired(string token)
        {
            var jwtToken = new JwtSecurityToken(token);
            return jwtToken.ValidTo < DateTime.UtcNow;
        }

        public async void Logout()
        {
            Token = string.Empty;
            await ProtectedLocalStorage.DeleteAsync("token");

            OpenLink("");
        }

        public void OpenLink(string url)
        {
            nv.NavigateTo(url, true);
        }


        public async Task RefreshPage()
        {

            try
            {
                await js.InvokeVoidAsync("RefreshMDInput");
                await InvokeAsync(StateHasChanged);
                await Task.Delay(500);
                await js.InvokeVoidAsync("RefreshMDInput");
                await InvokeAsync(StateHasChanged);
            }

            catch
            {

            }

        }



    }
}
