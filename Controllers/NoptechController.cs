using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Plugin.Widgets.Noptech.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.Noptech.Controllers
{
    public class NoptechController : BasePluginController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ILogger logger;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public NoptechController(ILocalizationService localizationService,
            ILogger logger,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
            IStoreService storeService,
            IWebHelper webHelper)
        {
            _localizationService = localizationService;
            this.logger = logger;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _storeService = storeService;
            _webHelper = webHelper;
        }

        #endregion

        #region Methods

        [Area(AreaNames.Admin)]
        [AuthorizeAdmin]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Configure()
        {
            this.logger.LogError("Test string for hitting the Action");

            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<NoptechSettings>(storeId);
            var widgetSettings = await _settingService.LoadSettingAsync<WidgetSettings>(storeId);

            var model = new ConfigurationModel
            {
                PictureUrl = settings.Link1,
                //Enabled = widgetSettings.ActiveWidgetSystemNames.Contains(NoptechDefaults.SystemName),
                ActiveStoreScopeConfiguration = storeId
            };

            if (storeId > 0)
            {
                model.Script_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.Link1, storeId);
                model.Enabled_OverrideForStore = await _settingService.SettingExistsAsync(widgetSettings, setting => setting.ActiveWidgetSystemNames, storeId);
            }

            //prepare store URL
            model.Url = storeId > 0
                ? (await _storeService.GetStoreByIdAsync(storeId))?.Url
                : _webHelper.GetStoreLocation();

            return View("~/Plugins/Widgets.Noptech/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<NoptechSettings>(storeId);
            var widgetSettings = await _settingService.LoadSettingAsync<WidgetSettings>(storeId);

            settings.Link1 = model.PictureUrl;
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.Link1, model.Script_OverrideForStore, storeId, false);

            if (model.Enabled && !widgetSettings.ActiveWidgetSystemNames.Contains(NoptechDefaults.SystemName))
                widgetSettings.ActiveWidgetSystemNames.Add(NoptechDefaults.SystemName);
            if (!model.Enabled && widgetSettings.ActiveWidgetSystemNames.Contains(NoptechDefaults.SystemName))
                widgetSettings.ActiveWidgetSystemNames.Remove(NoptechDefaults.SystemName);
            await _settingService.SaveSettingOverridablePerStoreAsync(widgetSettings, setting => setting.ActiveWidgetSystemNames, model.Enabled_OverrideForStore, storeId, false);

            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        #endregion
    }
}