#pragma checksum "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\Pages\Manufacturers.razor" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "7618e1f12ef8f867b171cd0de10e98683594a423"
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
using BlazorInputFile;

#line default
#line hidden
#nullable disable
#nullable restore
#line 11 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\_Imports.razor"
using EtkBlazorApp.Data;

#line default
#line hidden
#nullable disable
#nullable restore
#line 12 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\_Imports.razor"
using EtkBlazorApp.Services;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\Pages\Manufacturers.razor"
using EtkBlazorApp.DataAccess;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\Pages\Manufacturers.razor"
using EtkBlazorApp.Components;

#line default
#line hidden
#nullable disable
    [Microsoft.AspNetCore.Components.RouteAttribute("/manufacturers")]
    public partial class Manufacturers : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
        }
        #pragma warning restore 1998
#nullable restore
#line 79 "D:\Programming\CSharp\Blazor\EtkBlazorApp\EtkBlazorApp\Pages\Manufacturers.razor"
       

    List<ManufacturerModel> manufacturers = null;
    ManufacturerModel editManufacturer = null;

    private void EditManufacturer(ManufacturerModel manufacturer)
    {
        editManufacturer = manufacturer;
    }

    private async Task ConfirmManufacturerChanges(ManufacturerModel manufacturer)
    {
        await _database.SaveManufacturer(manufacturer);
        editManufacturer = null;
    }

    protected override async Task OnInitializedAsync()
    {
        manufacturers = await _database.GetManufacturers();
    }


#line default
#line hidden
#nullable disable
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private IDatabase _database { get; set; }
    }
}
#pragma warning restore 1591
