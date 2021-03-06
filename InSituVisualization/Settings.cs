﻿using InSituVisualization.Model;
using InSituVisualization.ViewModels;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;

namespace InSituVisualization
{
    // ReSharper disable once ClassNeverInstantiated.Global, Justification: IoC
    internal class Settings
    {
        internal class SettingsOptions
        {
            private const string SettingsCategory = "Performance Visualization";

            public string AppId
            {
                get => PerfVizSettingsStore.GetString(SettingsCategory, "AppId");
                set => PerfVizSettingsStore.SetString(SettingsCategory, "AppId", value);
            }

            public string ApiKey
            {
                get => PerfVizSettingsStore.GetString(SettingsCategory, "ApiKey");
                set => PerfVizSettingsStore.SetString(SettingsCategory, "ApiKey", value);
            }

            public int MaxPullingAmount
            {
                get => PerfVizSettingsStore.GetInt32(SettingsCategory, "MaxPullingAmount");
                set => PerfVizSettingsStore.SetInt32(SettingsCategory, "MaxPullingAmount", value);
            }

            private ShellSettingsManager ShellSettingsManager { get; } = new ShellSettingsManager(ServiceProvider.GlobalProvider);

            private WritableSettingsStore PerfVizSettingsStore
            {
                get
                {
                    var settingsStore = ShellSettingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
                    if (!settingsStore.CollectionExists(SettingsCategory))
                    {
                        settingsStore.CreateCollection(SettingsCategory);
                    }
                    return settingsStore;
                }
            }
        }


        public SettingsOptions Options { get; } = new SettingsOptions();


        public DetailWindowViewModel DetailWindowViewModel { get; set; } = new DetailWindowViewModel();

        public void SetDetailWindowContent(PerformanceInfo performanceInfo)
        {
            DetailWindowViewModel.PerformanceInfo = performanceInfo;
        }

    }
}
