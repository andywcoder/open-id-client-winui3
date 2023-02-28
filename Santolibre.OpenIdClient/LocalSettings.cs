using System.Text.Json;
using Windows.Storage;

namespace Santolibre.OpenIdClient
{
    public class LocalSettings
    {
        public IdentityInformation IdentityInformation { get; set; }

        public static LocalSettings Load()
        {
            LocalSettings localSettings;
            ApplicationData.Current.LocalSettings.Values.TryGetValue("LocalSettings", out object localSettingsObject);
            if (localSettingsObject is string localSettingsJson)
            {
                localSettings = JsonSerializer.Deserialize<LocalSettings>(localSettingsJson) ?? CreateDefault();
            }
            else
            {
                localSettings = CreateDefault();
                localSettings.Save();
            }
            return localSettings;
        }

        public void Save()
        {
            ApplicationData.Current.LocalSettings.Values["LocalSettings"] = JsonSerializer.Serialize(this);
        }

        public static LocalSettings CreateDefault()
        {
            return new LocalSettings();
        }
    }
}
