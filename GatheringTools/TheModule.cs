﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Blish_HUD.Modules;
using Blish_HUD.Modules.Managers;
using Blish_HUD.Settings;
using GatheringTools.LogoutOverlay;
using GatheringTools.Services;
using GatheringTools.ToolSearch.Controls;
using GatheringTools.ToolSearch.Model;
using GatheringTools.ToolSearch.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GatheringTools
{
    [Export(typeof(Module))]
    public class TheModule : Module
    {
        private static readonly Logger Logger = Logger.GetLogger<TheModule>();
        internal SettingsManager SettingsManager => ModuleParameters.SettingsManager;
        internal ContentsManager ContentsManager => ModuleParameters.ContentsManager;
        internal DirectoriesManager DirectoriesManager => ModuleParameters.DirectoriesManager;
        internal Gw2ApiManager Gw2ApiManager => ModuleParameters.Gw2ApiManager;

        [ImportingConstructor]
        public TheModule([Import("ModuleParameters")] ModuleParameters moduleParameters) : base(moduleParameters)
        {
        }

        protected override void DefineSettings(SettingCollection settings)
        {
            _settingService = new SettingService(settings);
        }

        protected override async Task LoadAsync()
        {
            _textureService    = new TextureService(ContentsManager);
            _reminderContainer = new ReminderContainer(_textureService, _settingService);

            if (_settingService.ReminderIsVisibleForSetupSetting.Value)
                ShowReminderAndResetRunningTime();
            else
                HideReminderAndResetRunningTime();

            _settingService.ReminderIsVisibleForSetupSetting.PropertyChanged += (s, e) =>
            {
                if (_settingService.ReminderIsVisibleForSetupSetting.Value)
                    ShowReminderAndResetRunningTime();
                else
                    HideReminderAndResetRunningTime();
            };

            _escKeyBinding           =  new KeyBinding(Keys.Escape);
            _escKeyBinding.Activated += OnEscKeyBindingActivated;
            _escKeyBinding.Enabled   =  true;

            _enterKeyBinding           =  new KeyBinding(Keys.Enter);
            _enterKeyBinding.Activated += OnEnterKeyBindingActivated;
            _enterKeyBinding.Enabled   =  true;

            _settingService.LogoutKeyBindingSetting.Value.Activated += OnLogoutKeyBindingActivated;
            _settingService.LogoutKeyBindingSetting.Value.Enabled   =  true;

            var allGatheringTools = await FileService.GetAllGatheringToolsFromFiles(ContentsManager, Logger);
            _allGatheringTools.AddRange(allGatheringTools);

            _toolSearchStandardWindow = new ToolSearchStandardWindow(_textureService, _settingService, _allGatheringTools, Gw2ApiManager, Logger)
            {
                Emblem        = _textureService.ToolSearchWindowEmblem, // hack: has to be first to prevent bug of emblem not being visible
                Title         = "Tools",
                Location      = new Point(300, 300),
                SavesPosition = true,
                Id            = "tool search window 6f48189f-0a38-4fad-bc6a-10d323e7f1c4",
                Parent        = GameService.Graphics.SpriteScreen,
            };

            _settingService.ToolSearchKeyBindingSetting.Value.Activated += async (s, e) => await _toolSearchStandardWindow.ToggleVisibility();
            _settingService.ToolSearchKeyBindingSetting.Value.Enabled   =  true;

            _cornerIconService = new CornerIconService(_settingService.ShowToolSearchCornerIconSetting, _toolSearchStandardWindow, _textureService);
        }

        protected override void Update(GameTime gameTime)
        {
            if (_settingService.ReminderIsVisibleForSetupSetting.Value)
                return;

            if (_reminderContainer.Visible)
            {
                _runningTime += gameTime.ElapsedGameTime.TotalMilliseconds;

                if (_runningTime > 1000 * (int)_settingService.ReminderDisplayDurationInSecondsSetting.Value)
                    HideReminderAndResetRunningTime();
            }
        }

        protected override void Unload()
        {
            _escKeyBinding.Activated                                -= OnEscKeyBindingActivated;
            _settingService.LogoutKeyBindingSetting.Value.Activated -= OnLogoutKeyBindingActivated;

            _textureService?.Dispose();
            _toolSearchStandardWindow?.Dispose();
            _reminderContainer?.Dispose();
            _cornerIconService?.RemoveCornerIcon();
        }

        private void OnEscKeyBindingActivated(object sender, EventArgs e)
        {
            if (_settingService.ReminderIsVisibleForSetupSetting.Value)
                return;

            if (_reminderContainer.Visible == false)
                return;

            if (_settingService.EscIsHidingReminderSetting.Value)
                HideReminderAndResetRunningTime();
        }

        private void OnEnterKeyBindingActivated(object sender, EventArgs e)
        {
            if (_settingService.ReminderIsVisibleForSetupSetting.Value)
                return;

            if (_reminderContainer.Visible == false)
                return;

            if (_settingService.EnterIsHidingReminderSetting.Value)
                HideReminderAndResetRunningTime();
        }

        private void OnLogoutKeyBindingActivated(object sender, EventArgs e)
        {
            if (_reminderContainer.Visible == false)
                ShowReminderAndResetRunningTime();
        }

        private void HideReminderAndResetRunningTime()
        {
            _runningTime = 0;
            _reminderContainer.Hide();
        }

        private void ShowReminderAndResetRunningTime()
        {
            _runningTime = 0;
            _reminderContainer.Show();

            if(_settingService.ReminderScreenNotificationIsEnabled.Value)
                ScreenNotification.ShowNotification(_settingService.ReminderTextSetting.Value, ScreenNotification.NotificationType.Error);
        }

        private double _runningTime;
        private ToolSearchStandardWindow _toolSearchStandardWindow;
        private ReminderContainer _reminderContainer;
        private KeyBinding _escKeyBinding;
        private KeyBinding _enterKeyBinding;
        private SettingService _settingService;
        private TextureService _textureService;
        private CornerIconService _cornerIconService;
        private readonly List<GatheringTool> _allGatheringTools = new List<GatheringTool>();
    }
}