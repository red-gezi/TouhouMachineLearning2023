// <auto-generated/>
#pragma warning disable 1591
#pragma warning disable 0414
#pragma warning disable 0649
#pragma warning disable 0169

namespace Server.Shared
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
#nullable restore
#line 1 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\_Imports.razor"
using System.Net.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\_Imports.razor"
using Microsoft.AspNetCore.Authorization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\_Imports.razor"
using Microsoft.AspNetCore.Components.Authorization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\_Imports.razor"
using Microsoft.AspNetCore.Components.Forms;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\_Imports.razor"
using Microsoft.AspNetCore.Components.Routing;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\_Imports.razor"
using Microsoft.AspNetCore.Components.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\_Imports.razor"
using Microsoft.AspNetCore.Components.Web.Virtualization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\_Imports.razor"
using Microsoft.JSInterop;

#line default
#line hidden
#nullable disable
#nullable restore
#line 9 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\_Imports.razor"
using Server;

#line default
#line hidden
#nullable disable
#nullable restore
#line 10 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\_Imports.razor"
using Server.Shared;

#line default
#line hidden
#nullable disable
#nullable restore
#line 11 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\_Imports.razor"
using AntDesign;

#line default
#line hidden
#nullable disable
#nullable restore
#line 12 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\_Imports.razor"
using AntDesign.Charts;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\Shared\MainLayout.razor"
using MongoDB.Driver;

#line default
#line hidden
#nullable disable
    public partial class MainLayout : LayoutComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
        }
        #pragma warning restore 1998
#nullable restore
#line 6 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\Shared\MainLayout.razor"
       
	bool isLogin = false;
	string input_user = "";
	string input_password = "";
	public static PlayerInfo? userInfo = null;

#line default
#line hidden
#nullable disable
#nullable restore
#line 37 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\Shared\MainLayout.razor"
       
	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender)
		{
			CheckUserInfo();
		}
	}
	async void CheckUserInfo()
	{
		if (await localStorage.ContainKeyAsync("user"))
		{
			string user = await localStorage.GetItemAsync<string>("user");
			string password = await localStorage.GetItemAsync<string>("password");
			userInfo = MongoDbCommand.Login(input_user, input_password);
			if (userInfo != null)
			{
				isLogin = true;
				StateHasChanged();
				//await message.Success("欢迎回来"+userInfo.Name);
			}
		}
	}
	async void Login()
	{
		userInfo = MongoDbCommand.Login(input_user, input_password);
		if (userInfo != null)
		{
			isLogin = true;
			await localStorage.SetItemAsync("user", input_user);
			await localStorage.SetItemAsync("password", input_password);
			await localStorage.SetItemAsync("uid", userInfo.UID);
			await message.Success("登陆成功");
		}
		else
		{
			await message.Error("登陆失败");
		}
	}
	async void Logout()
	{
		userInfo = null;
		isLogin = false;
		await localStorage.RemoveItemAsync("user");
		await localStorage.RemoveItemAsync("password");
		await localStorage.RemoveItemAsync("uid");
		await message.Warning("退出登录状态");
	}

#line default
#line hidden
#nullable disable
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private Blazored.LocalStorage.ILocalStorageService localStorage { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private MessageService message { get; set; }
    }
}
#pragma warning restore 1591
