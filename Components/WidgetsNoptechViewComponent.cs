using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.Noptech.Components
{
    [ViewComponent(Name = "WidgetsNoptech")]
    public class WidgetsNoptechViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Widgets.Noptech/Views/NoptechInfo.cshtml");
        }
    }
}
