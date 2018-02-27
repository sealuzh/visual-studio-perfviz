using System;
using InSituVisualization.ViewModels;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;

namespace InSituVisualization
{
    // ReSharper disable once ClassNeverInstantiated.Global, Justification: IoC
    internal class Settings
    {
        public IPerformanceInfoDetailWindow PerformanceInfoDetailWindow { get; set; }

        public string AppId
        {
            get => (string)GetWritableSettingsStoreValue("Performance Visualization", "AppId", typeof(string));
            set => SetWritableSettingsStoreValue("Performance Visualization", "AppId", value);
        }

        public string ApiKey
        {
            get => (string)GetWritableSettingsStoreValue("Performance Visualization", "ApiKey", typeof(string));
            set => SetWritableSettingsStoreValue("Performance Visualization", "ApiKey", value);
        }

        public int MaxPullingAmount
        {
            get => (int)GetWritableSettingsStoreValue("Performance Visualization", "MaxPullingAmount", typeof(int));
            set => SetWritableSettingsStoreValue("Performance Visualization", "MaxPullingAmount", value);
        }

        //TODO: Overload
        public void SetWritableSettingsStoreValue(string category, string name, object value)
        {
            if (!GetWritableSettingsStore().PropertyExists(category, name))
            {
                GetWritableSettingsStore().CreateCollection(category);
            }

            switch (value.GetType().ToString())
            {
                case "System.String":
                    GetWritableSettingsStore().SetString(category, name, (string)value);
                    break;
                case "System.Int32":
                    GetWritableSettingsStore().SetInt32(category, name, (int)value);
                    break;
                default:
                    break;
            }
        }

        public object GetWritableSettingsStoreValue(string category, string name, Type filetype)
        {
            if (GetWritableSettingsStore().PropertyExists(category, name))
            {
                switch (filetype.ToString())
                {
                    case "System.String":
                        return GetWritableSettingsStore().GetString(category, name);
                    case "System.Int32":
                        return GetWritableSettingsStore().GetInt32(category, name);
                    default:
                        return null;
                }
            }
            else
            {
                switch (filetype.ToString())
                {
                    case "System.String":
                        return null;
                    case "System.Int32":
                        return 0;
                    default:
                        return null;
                }

            }

        }

        private static WritableSettingsStore GetWritableSettingsStore()
        {
            var shellSettingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
            return shellSettingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
        }

    }
}
