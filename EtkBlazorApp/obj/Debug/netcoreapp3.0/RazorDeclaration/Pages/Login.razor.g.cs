// <auto-generated/>
#pragma warning disable 1591
#pragma warning disable 0414
#pragma warning disable 0649
#pragma warning disable 0169

namespace EtkBlazorApp.Pages
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
#nullable restore
#line 1 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\_Imports.razor"
using System.Net.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\_Imports.razor"
using Microsoft.AspNetCore.Authorization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\_Imports.razor"
using Microsoft.AspNetCore.Components.Authorization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\_Imports.razor"
using Microsoft.AspNetCore.Components.Forms;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\_Imports.razor"
using Microsoft.AspNetCore.Components.Routing;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\_Imports.razor"
using Microsoft.AspNetCore.Components.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\_Imports.razor"
using Microsoft.JSInterop;

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\_Imports.razor"
using EtkBlazorApp;

#line default
#line hidden
#nullable disable
#nullable restore
#line 9 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\_Imports.razor"
using EtkBlazorApp.Shared;

#line default
#line hidden
#nullable disable
#nullable restore
#line 10 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\_Imports.razor"
using EtkBlazorApp.Data;

#line default
#line hidden
#nullable disable
#nullable restore
#line 11 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\_Imports.razor"
using EtkBlazorApp.Services;

#line default
#line hidden
#nullable disable
    [Microsoft.AspNetCore.Components.RouteAttribute("/login")]
    public partial class Login : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
        }
        #pragma warning restore 1998
#nullable restore
#line 31 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\Pages\Login.razor"
       

    private UserModel user;

    protected override void OnInitialized()
    {
        user = new UserModel();
    }

    private async Task<bool> ValidateUser()
    {
        await ((MyCustomAuthProvider)authenticationStateProvider).AuthenticateUser(user);
        navigation.NavigateTo("/");

        await session.SetItemAsync<string>("user_login", user.Login);
        await session.SetItemAsync<string>("user_password", user.Password);

        return await Task.FromResult(true);
    }


#line default
#line hidden
#nullable disable
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private Blazored.SessionStorage.ISessionStorageService session { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private NavigationManager navigation { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private AuthenticationStateProvider authenticationStateProvider { get; set; }
    }
}
#pragma warning restore 1591
