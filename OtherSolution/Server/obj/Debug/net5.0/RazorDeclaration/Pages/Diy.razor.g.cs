// <auto-generated/>
#pragma warning disable 1591
#pragma warning disable 0414
#pragma warning disable 0649
#pragma warning disable 0169

namespace Server.Pages
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
#line 2 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\Pages\Diy.razor"
using System.IO;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\Pages\Diy.razor"
using Newtonsoft.Json;

#line default
#line hidden
#nullable disable
    [Microsoft.AspNetCore.Components.RouteAttribute("/Diy")]
    public partial class Diy : Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
        }
        #pragma warning restore 1998
#nullable restore
#line 14 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\Pages\Diy.razor"
       
	public int currentPage = 0;
	public int pageMaxCount = 3;
	DiyCardInfo? ClickCard { get; set; }
	List<DiyCardInfo> DiyCardsInfo = DiyCommand.GetDiyCardsInfo();
	List<DiyCardInfo> OrderDiyCardsInfo = DiyCommand.GetDiyCardsInfo();
	List<DiyCardInfo> ShowDiyCardsInfo => OrderDiyCardsInfo.Skip(pageMaxCount*(currentPage-1)).Take(pageMaxCount).ToList();
	public int OrderType = 0;
	public void OrderByHot()
	{
		message.Info("按热度排序");
		OrderDiyCardsInfo=DiyCommand.GetDiyCardsInfo().OrderBy(x=>x.CardName).ToList();
	}
	public void OrderByLike()
	{
		message.Info("按赞同度排序");
		OrderDiyCardsInfo=DiyCommand.GetDiyCardsInfo().OrderBy(x=>x.Point).ToList();
	}
	public void OrderByTime()
	{
		message.Info("按时间排序");
		OrderDiyCardsInfo=DiyCommand.GetDiyCardsInfo().OrderBy(x=>x.Camp).ToList();
	}
	void OnChangeSize()
	{
		switch (OrderType)
		{
			case (0): OrderByHot(); break;
			case (1): OrderByLike(); break;
			case (2): OrderByTime(); break;
		}
		StateHasChanged();
	}
	void OnPageChange(PaginationEventArgs args)
	{
		currentPage = args.Page;
	}
	async Task<int> GetUserUid()
	{
		if(await localStorage.ContainKeyAsync("uid"))
		{
			return await localStorage.GetItemAsync<int>("uid");
		}
		else
		{
			return 0;
		}
	}

#line default
#line hidden
#nullable disable
#nullable restore
#line 133 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\Pages\Diy.razor"
       
    string UploadMode{ get; set; }

    string NewDiyCardName { get; set; }
    string NewDiyCardType { get; set; }
    int NewDiyCardPoint { get; set; }
    string NewDiyCardTags { get; set; }
    string NewDiyCardRank { get; set; }
    string NewDiyCardCamp { get; set; }
    string NewDiyCardDescribe { get; set; }
    string NewDiyCardAbility { get; set; }

    string HasShowPermission;
    string HasUsePermission;

    void UploadModeChange(string select)
    {
        UploadMode = select;
        StateHasChanged();
    }
    void ShowPermissionChange(string select) => HasShowPermission = select;
    void UsePermissionChange(string select) => HasUsePermission = select;

    void CardTypeChange(string select) => NewDiyCardType = select;
    void CardRankChange(string select) => NewDiyCardRank = select;
    void CardCampChange(string select) => NewDiyCardCamp = select;

    bool diyDrawerVisible = false;
    bool detailPageVisible = false;

	void DiyDrawerOpen() => this.diyDrawerVisible = true;
	void DiyDrawerClose() => this.diyDrawerVisible = false;
	void DiyDrawerInit()
	{
		NewDiyCardName = "";
		NewDiyCardDescribe = "";
	}

	byte[] uploadImageData = new byte[0];
	public async Task UpLoadImage(InputFileChangeEventArgs e)
	{
		int fileLength = (int)e.File.Size;
		uploadImageData = new byte[fileLength];
		await e.File.OpenReadStream().ReadAsync(uploadImageData, 0, fileLength);
	}
	public async Task SubmitCard()
	{
		string path = $"{DateTime.Now.Millisecond}-{NewDiyCardName}.png";
		File.WriteAllBytes($"wwwroot\\{path}", uploadImageData);
		DiyCommand.AddDiyCardInfos(NewDiyCardName, NewDiyCardTags, NewDiyCardPoint, NewDiyCardRank, NewDiyCardCamp, NewDiyCardDescribe, NewDiyCardAbility, path);
		DiyCardsInfo = DiyCommand.GetDiyCardsInfo();
		await message.Success("上传完成");
	}

#line default
#line hidden
#nullable disable
#nullable restore
#line 256 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\Pages\Diy.razor"
       
	bool commitDrawerVisible = false;
	void OpenCommitDrawer() => this.commitDrawerVisible = true;
	void CloseCommitDrawer() => this.commitDrawerVisible = false;
	bool like = false;
	bool dislike = false;

	RenderFragment likeAction =>
	

#line default
#line hidden
#nullable disable
        (__builder2) => {
            __builder2.OpenElement(0, "span");
            __builder2.AddMarkupContent(1, "\r\n\t\t\t\t\t\t\t\t");
            __builder2.OpenElement(2, "Tooltip");
            __builder2.AddAttribute(3, "Title", 
#nullable restore
#line 265 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\Pages\Diy.razor"
                                                  "Like"

#line default
#line hidden
#nullable disable
            );
            __builder2.AddMarkupContent(4, "\r\n\t\t\t\t\t\t\t\t\t");
            __builder2.OpenElement(5, "Icon");
            __builder2.AddAttribute(6, "Type", "like");
            __builder2.AddAttribute(7, "Theme", 
#nullable restore
#line 266 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\Pages\Diy.razor"
                                                               like ? "fill" : "outline"

#line default
#line hidden
#nullable disable
            );
            __builder2.AddAttribute(8, "OnClick", "SetLike");
            __builder2.CloseElement();
            __builder2.AddMarkupContent(9, "\r\n\t\t\t\t\t\t\t\t");
            __builder2.CloseElement();
            __builder2.AddMarkupContent(10, "\r\n\t\t\t\t\t\t\t\t");
            __builder2.OpenElement(11, "span");
#nullable restore
#line 268 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\Pages\Diy.razor"
__builder2.AddContent(12, like ? 1 : 0);

#line default
#line hidden
#nullable disable
            __builder2.CloseElement();
            __builder2.AddMarkupContent(13, "\r\n\t");
            __builder2.CloseElement();
        }
#nullable restore
#line 269 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\Pages\Diy.razor"
           ;

	RenderFragment dislikeAction =>
	

#line default
#line hidden
#nullable disable
        (__builder2) => {
            __builder2.OpenElement(14, "span");
            __builder2.AddMarkupContent(15, "\r\n\t\t\t\t\t\t\t\t");
            __builder2.OpenElement(16, "Tooltip");
            __builder2.AddAttribute(17, "Title", 
#nullable restore
#line 273 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\Pages\Diy.razor"
                                                  "Dislike"

#line default
#line hidden
#nullable disable
            );
            __builder2.AddMarkupContent(18, "\r\n\t\t\t\t\t\t\t\t\t");
            __builder2.OpenElement(19, "Icon");
            __builder2.AddAttribute(20, "Type", "dislike");
            __builder2.AddAttribute(21, "Theme", 
#nullable restore
#line 274 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\Pages\Diy.razor"
                                                                  dislike ? "fill" : "outline"

#line default
#line hidden
#nullable disable
            );
            __builder2.AddAttribute(22, "OnClick", "SetDislike");
            __builder2.CloseElement();
            __builder2.AddMarkupContent(23, "\r\n\t\t\t\t\t\t\t\t");
            __builder2.CloseElement();
            __builder2.AddMarkupContent(24, "\r\n\t\t\t\t\t\t\t\t");
            __builder2.OpenElement(25, "span");
#nullable restore
#line 276 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\Pages\Diy.razor"
__builder2.AddContent(26, dislike ? 1 : 0);

#line default
#line hidden
#nullable disable
            __builder2.CloseElement();
            __builder2.AddMarkupContent(27, "\r\n\t");
            __builder2.CloseElement();
        }
#nullable restore
#line 277 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\Pages\Diy.razor"
           ;

	RenderFragment dateTime =
	

#line default
#line hidden
#nullable disable
        (__builder2) => {
            __builder2.OpenElement(28, "Tooltip");
            __builder2.AddAttribute(29, "Title", 
#nullable restore
#line 280 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\Pages\Diy.razor"
                       DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")

#line default
#line hidden
#nullable disable
            );
            __builder2.AddMarkupContent(30, "\r\n\t\t\t");
            __builder2.OpenElement(31, "span");
            __builder2.AddMarkupContent(32, "\r\n\t\t\t");
#nullable restore
#line 282 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\Pages\Diy.razor"
__builder2.AddContent(33, MomentHelper.FromNow(DateTime.Now));

#line default
#line hidden
#nullable disable
            __builder2.AddMarkupContent(34, "\r\n\t\t\t");
            __builder2.CloseElement();
            __builder2.AddMarkupContent(35, "\r\n\t");
            __builder2.CloseElement();
        }
#nullable restore
#line 284 "G:\UnityProject\TouhouMachineLearning2022\OtherSolution\Server\Pages\Diy.razor"
              ;
	async void onSubmit()
	{

	}
	void SetLike()
	{
		like = true;
		dislike = false;
	}

	void SetDislike()
	{
		like = false;
		dislike = true;
	}

#line default
#line hidden
#nullable disable
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private Blazored.LocalStorage.ILocalStorageService localStorage { get; set; }
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private MessageService message { get; set; }
    }
}
#pragma warning restore 1591
