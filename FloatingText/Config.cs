using ItemDecoration;
using Newtonsoft.Json;
using TShockAPI;

namespace FloatingText
{
    public class FloatingTextConfig
    {
        public GeneralSettings General { get; set; } = new GeneralSettings();
        public FilterSettings Filters { get; set; } = new FilterSettings();
        public SoundSettings Sound { get; set; } = new SoundSettings();

        public static FloatingTextConfig Load(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    throw new ArgumentException("La ruta del archivo de configuración no puede ser nula o vacía.");
                }

                var directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    TShock.Log.ConsoleInfo($"[FloatingText] Carpeta de configuración creada: {directory}");
                }

                if (!File.Exists(path))
                {
                    var defaultConfig = new FloatingTextConfig();
                    defaultConfig.Save(path);
                    TShock.Log.ConsoleInfo($"[FloatingText] Archivo de configuración creado: {path}");
                    return defaultConfig;
                }

                string jsonContent = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<FloatingTextConfig>(jsonContent);
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"[FloatingText] Error al cargar la configuración: {ex.Message}");
                TShock.Log.ConsoleError("[FloatingText] Se generará un archivo de configuración predeterminado.");
                Telemetry.Report(ex);
                var fallbackConfig = new FloatingTextConfig();
                fallbackConfig.Save(path);
                return fallbackConfig;
            }
        }

        public void Save(string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    throw new ArgumentException("La ruta del archivo de configuración no puede ser nula o vacía.");
                }

                string jsonContent = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(path, jsonContent);
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"[FloatingText] Error al guardar la configuración: {ex.Message}");
                Telemetry.Report(ex);
            }
        }
    }

    public class GeneralSettings
    {
        public bool EnableInitializationRequest { get; set; } = true;
        public List<string> ExcludedGroups { get; set; } = new List<string> { "Guest" };
        public bool RequireRegistration { get; set; } = true;
    }

    public class FilterSettings
    {
        public bool PlayerNotDead { get; set; } = true;
        public bool RequirePermission { get; set; } = false;
        public string Permission { get; set; } = "floatingtext.show";
    }

    public class SoundSettings
    {
        public float Volume { get; set; } = 1.0f;
    }
}