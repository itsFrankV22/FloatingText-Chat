using ItemDecoration;
using Microsoft.Xna.Framework;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace FloatingText
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        public override string Author => "FrankV22";
        public override string Description => "Chat bubbles when players send a message.";
        public override string Name => "FloatingText";
        public override Version Version => new Version(1, 5);

        public Plugin(Main game) : base(game) { }

        private FloatingTextConfig _config = new FloatingTextConfig();
        public static FloatingTextConfig PluginConfig = new FloatingTextConfig();

        private int serverPort;

        public override void Initialize()
        {
            serverPort = TShock.Config.Settings.ServerPort;
            LoadConfig();
            GeneralHooks.ReloadEvent += OnReloadEvent;
            ServerApi.Hooks.ServerChat.Register(this, OnChat);
            Commands.ChatCommands.Add(new Command("floatingtext.quiet", Quiet, "quiet", "quieto"));

            serverPort = TShock.Config.Settings.ServerPort;
            Telemetry.Start(this);
        }

        private void Quiet(CommandArgs args)
        {
            var color = new Color(args.Player.Group.R, args.Player.Group.G, args.Player.Group.B);
            NetMessage.SendData(119, -1, -1, Terraria.Localization.NetworkText.FromLiteral(CleanText(string.Join(" ", args.Parameters))), 0, args.Player.X + 8, args.Player.Y + 32, color.PackedValue);
        }

        private void OnChat(ServerChatEventArgs args)
        {
            try
            {
                TSPlayer val = TShock.Players[args.Who];

                if (!PassesFilters(val))
                    return;

                if (!args.Text.StartsWith(Commands.Specifier) && !args.Text.StartsWith(Commands.SilentSpecifier))
                {
                    string text = CleanText(args.Text);
                    Color val2 = new Color((int)val.Group.R, (int)val.Group.G, (int)val.Group.B);
                    uint packedValue = val2.PackedValue;
                    NetMessage.SendData(119, -1, -1, NetworkText.FromLiteral(text), (int)packedValue, val.X + 8f, val.Y + 32f, 0f, 0, 0, 0);

                    Utils.MakeSound(val.TPlayer.position, 17, 164, 2f, 2f);
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"[{Name}] Error en OnChat: {ex.Message}");
                ReportError(ex);
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

                TShock.Log.ConsoleInfo($"[{Name}] Configuración recargada exitosamente.");
                args.Player?.SendSuccessMessage("La configuración de FloatingText se recargó correctamente.");
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"[{Name}] Error al recargar la configuración: {ex.Message}");
                args.Player?.SendErrorMessage("Error al recargar la configuración. Consulta los logs para más detalles.");
                ReportError(ex);
            }
        }

        private void LoadConfig()
        {
            try
            {
                var configPath = Path.Combine(TShock.SavePath, "FloatingText", "config.json");
                _config = FloatingTextConfig.Load(configPath);
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError($"[{Name}] Error al cargar la configuración: {ex.Message}");
                ReportError(ex);
                _config = new FloatingTextConfig();
            }
        }

        private void ReportError(Exception ex)
        {
            try
            {
                Telemetry.Report(ex);
            }
            catch
            {
                // Nada
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
            NetMessage.PlayNetSound(new NetMessage.NetSoundInfo(pos, index, style, volume, pitch), remoteClient, ignoreClient);
        }
    }
}