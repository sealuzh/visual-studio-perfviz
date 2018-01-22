using System;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;

namespace InSituVisualization.TelemetryCollector
{
    static class WritableSettingsStoreController
    {
        //TODO: Overload
        public static void SetWritableSettingsStoreValue(string category, string name, object value)
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

        public static object GetWritableSettingsStoreValue(string category, string name, Type filetype)
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
