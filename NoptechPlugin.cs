namespace Nop.Plugin.Widgets.Noptech
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.Extensions.FileProviders;
    using Nop.Core;
    using Nop.Core.Infrastructure;
    using Nop.Services.Cms;
    using Nop.Services.Configuration;
    using Nop.Services.Localization;
    using Nop.Services.Media;
    using Nop.Services.Plugins;
    using Nop.Services.Stores;
    using Nop.Web.Framework.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class NoptechPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly INopFileProvider _nopFileProvider;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;

        public NoptechPlugin(
            INopFileProvider nopFileProvider,
            IPictureService pictureService,
            ISettingService settingService,
            ILocalizationService localizationService,
            IWebHelper webHelper)
        {
            _nopFileProvider = nopFileProvider;
            _pictureService = pictureService;
            _settingService = settingService;
            _localizationService = localizationService;
            _webHelper = webHelper;
        }


        public bool HideInWidgetList => false;

        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/WidgetsNoptech/Configure";
        }

        public string GetWidgetViewComponentName(string widgetZone)
        {
            if (widgetZone == null)
                throw new ArgumentNullException(nameof(widgetZone));

            return NoptechDefaults.VIEW_COMPONENT;
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { "" });
        }

        public override async Task InstallAsync()
        {
            var sampleImagesPath = _nopFileProvider
                .MapPath("~/Plugins/Widgets.Noptech/Content/noptech/sample-images/");

            var picture = await _pictureService
                .InsertPictureAsync(await _nopFileProvider.ReadAllBytesAsync(_nopFileProvider.Combine(sampleImagesPath, "banner1.jpg")), MimeTypes.ImagePJpeg, "banner_1");

            NoptechSettings settings = new ()
            {
                Picture1Id = picture.Id,
                Text1 = "",
                Link1 = _webHelper.GetStoreLocation(),
                WidgetZone = PublicWidgetZones.HeadHtmlTag
            };

            await _settingService.SaveSettingAsync(settings);

            //await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            //{
            //    ["Plugins.Widgets.Noptech.Fields.Enabled"] = "Enable",
            //    ["Plugins.Widgets.Noptech.Fields.Enabled.Hint"] = "Check to activate this widget.",
            //    ["Plugins.Widgets.Noptech.Fields.Script"] = "Installation script",
            //    ["Plugins.Widgets.Noptech.Fields.Script.Hint"] = "Find your unique installation script on the Installation tab in your account and then copy it into this field.",
            //    ["Plugins.Widgets.Noptech.Fields.Script.Required"] = "Installation script is required",
            //});

            await base.InstallAsync();
        }

        public async override Task PreparePluginToUninstallAsync()
        {
            await base.PreparePluginToUninstallAsync();
        }

        public async override Task UninstallAsync()
        {
            await base.UninstallAsync();
        }

        public async override Task UpdateAsync(string currentVersion, string targetVersion)
        {
            await base.UpdateAsync(currentVersion, targetVersion);
        }
    }
}