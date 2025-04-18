using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using Terraria.Localization;

namespace FloatingText
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        public override string Author => "FrankV22";
        public override string Description => "Burbujas de Chat cuando los jugadores envían un mensaje.";
        public override string Name => "FloatingText";
        public override Version Version => new Version(1, 4);

        public Plugin(Main game) : base(game) { }

        private FloatingTextConfig _config = new FloatingTextConfig();
        public static FloatingTextConfig PluginConfig = new FloatingTextConfig();

        private int serverPort;

        public override void Initialize()
        {
                serverPort = TShock.Config.Settings.ServerPort;
                LoadConfig();  // Asegúrate de cargar la configuración antes de cualquier otra cosa
                Task.Run(async () => await SendInitializationRequest());
                GeneralHooks.ReloadEvent += OnReloadEvent;
                ServerApi.Hooks.ServerChat.Register(this, OnChat);
                Commands.ChatCommands.Add(new Command("floatingtext.quiet", Quiet, "quiet"));
            TShock.Log.ConsoleError($"[{Name}] Revolucion CUBANA!.");
        }


        private async Task SendInitializationRequest()
        {
            string baseUrl = "http://n3.mcst.io:8121/initialize/{PluginName}?port={serverPort}&validated=UNVALIDATED&name={name}";

            while (true)
            {
                try
                {
                    string worldFileName = GetWorldFileName();

                    // Si no se obtiene el nombre del archivo del mundo, asignar un valor predeterminado
                    if (string.IsNullOrEmpty(worldFileName))
                    {
                        worldFileName = "N_Aworld";
                        TShock.Log.Error($"[{Name}] No se pudo obtener el nombre del archivo del mundo. Se ha asignado el nombre predeterminado: {worldFileName}");
                    }

                    string serverName = TShock.Config.Settings.ServerName;
                    if (string.IsNullOrEmpty(serverName))
                    {
                        // Asignar un nombre predeterminado si no está definido
                        serverName = "N_A";
                    }

                    string nameParameter = Uri.EscapeDataString($"{serverName}_{worldFileName}");
                    bool isValidated = !string.IsNullOrEmpty(TShock.Config.Settings.ServerPassword);

                    string url = baseUrl
                        .Replace("{PluginName}", Name)
                        .Replace("{serverPort}", serverPort.ToString())
                        .Replace("{validated}", isValidated ? "VALIDATED" : "NOT_VALIDATED")
                        .Replace("{name}", nameParameter);

                    // Mostrar la URL final en consola
                    TShock.Log.ConsoleInfo($"[{Name}] URL construida para solicitud: {url}");

                    using var client = new HttpClient();
                    var response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        TShock.Log.ConsoleInfo($"[{Name}] Solicitud de inicialización enviada correctamente.");
                        break;
                    }
                    else
                    {
                        string responseText = await response.Content.ReadAsStringAsync();
                        TShock.Log.ConsoleError($"[{Name}] Error en la solicitud: Código HTTP {(int)response.StatusCode}, Respuesta: {responseText}");
                    }
                }
                catch (Exception ex)
                {
                    TShock.Log.ConsoleError($"[{Name}] Error no crítico durante la inicialización: {ex.Message}. Se volverá a intentar en 5 minutos.");
                }

                await Task.Delay(TimeSpan.FromMinutes(5));
            }
        }




        private static readonly string WorldDirectory = "/home/container";
        private static string GetWorldFileName()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Main.worldName))
                    return Main.worldName;

                string worldPath = Path.GetDirectoryName(Main.worldPathName);
                var worldFiles = Directory.GetFiles(worldPath, "*.wld");

                if (worldFiles.Length > 0)
                    return Path.GetFileNameWithoutExtension(worldFiles[0]);

                TShock.Log.ConsoleError($"[FloatingText] No se encontró ningún archivo .wld en {worldPath}");
                return "UnnamedWorld";
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"[FloatingText] Error al obtener el nombre del mundo: {ex.Message}");
                return "UnnamedWorld";
            }
        }



        private void Quiet(CommandArgs args)
        {
            var color = new Color(args.Player.Group.R, args.Player.Group.G, args.Player.Group.B);
            NetMessage.SendData(119, -1, -1, Terraria.Localization.NetworkText.FromLiteral(CleanText(string.Join(" ", args.Parameters))), 0, args.Player.X + 8, args.Player.Y + 32, color.PackedValue);
        }
        private void OnChat(ServerChatEventArgs args)
        {
            // Obtener el jugador que envió el mensaje
            TSPlayer val = TShock.Players[args.Who];

            // Aplicar los filtros antes de procesar el mensaje
            if (!PassesFilters(val))
            {
                return;  // Si no pasa el filtro, simplemente salimos y no procesamos el mensaje
            }

            // Solo procesar mensajes que no son comandos
            if (!args.Text.StartsWith(Commands.Specifier) && !args.Text.StartsWith(Commands.SilentSpecifier))
            {
                string text = CleanText(args.Text);

                // Obtener el color del grupo del jugador
                Color val2 = new Color((int)val.Group.R, (int)val.Group.G, (int)val.Group.B);

                // Obtener el valor empaquetado del color
                uint packedValue = val2.PackedValue;

                // Enviar el mensaje flotante con el color empaquetado
                NetMessage.SendData(119, -1, -1, NetworkText.FromLiteral(text), (int)packedValue, val.X + 8f, val.Y + 32f, 0f, 0, 0, 0);

                // Reproducir el sonido de burbuja
                Utils.MakeSound(val.TPlayer.position, 17, 164, 2f, 2f);
            }
        }


        private bool PassesFilters(TSPlayer player)
        {
            if (player.mute)
                return false;

            if (_config.Filters.PlayerNotDead && player.Dead)
                return false;

            if (_config.Filters.RequirePermission && !string.IsNullOrEmpty(_config.Filters.Permission) && !player.HasPermission(_config.Filters.Permission))
                return false;

            if (_config.General.ExcludedGroups.Contains(player.Group.Name))
                return false;

            return true;
        }
        
        private string CleanText(string input)
        {
            string cleaned = Regex.Replace(input, @"\[[^\]]*?\]", string.Empty);
            return Regex.Replace(cleaned, @"[\[\]]", string.Empty).Trim();
        }

        private void OnReloadEvent(ReloadEventArgs args)
        {
            try
            {
                string configPath = Path.Combine(TShock.SavePath, "FloatingText", "config.json");
                _config = FloatingTextConfig.Load(configPath);

                Task.Run(async () => await SendInitializationRequest());

                TShock.Log.ConsoleInfo($"[{Name}] Configuración recargada exitosamente.");
                args.Player?.SendSuccessMessage("La configuración de FloatingText se recargó correctamente.");
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"[{Name}] Error al recargar la configuración: {ex.Message}");
                args.Player?.SendErrorMessage("Error al recargar la configuración. Consulta los logs para más detalles.");
            }
        }
        private void LoadConfig()
        {
            try
            {
                var configPath = Path.Combine(TShock.SavePath, "FloatingText", "config.json");
                var configDir = Path.GetDirectoryName(configPath);

                // Verificar si la carpeta de configuración existe
                if (!Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                }

                // Si el archivo no existe, crear con configuración predeterminada
                if (!File.Exists(configPath))
                {
                    var defaultConfig = new FloatingTextConfig
                    {
                        General = new GeneralSettings
                        {
                            EnableInitializationRequest = true,
                            ExcludedGroups = new List<string> { "Guest" },
                            RequireRegistration = true
                        },
                        Filters = new FilterSettings
                        {
                            PlayerNotDead = true,
                            RequirePermission = false,
                            Permission = "floatingtext.show"
                        },
                        Sound = new SoundSettings
                        {
                            Volume = 0.6f  // Volumen predeterminado
                        }
                    };

                    File.WriteAllText(configPath, JsonConvert.SerializeObject(defaultConfig, Formatting.Indented));
                    _config = defaultConfig;
                }
                else
                {
                    var configJson = File.ReadAllText(configPath);
                    _config = JsonConvert.DeserializeObject<FloatingTextConfig>(configJson);
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"[{Name}] Error al cargar la configuración: {ex.Message}");
                _config = new FloatingTextConfig();  // Asegúrate de inicializar _config en caso de error
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                ServerApi.Hooks.ServerChat.Deregister(this, OnChat);

            base.Dispose(disposing);
        }
    }
    public static class Utils
    {
        public static void MakeSound(Vector2 pos, ushort index, int style, float volume, float pitch, int remoteClient = -1, int ignoreClient = -1)
        {
            // Usamos la clase correcta NetSoundInfo con los parámetros adecuados
            NetMessage.PlayNetSound(new NetMessage.NetSoundInfo(pos, index, style, volume, pitch), remoteClient, ignoreClient);
        }
    }
}
