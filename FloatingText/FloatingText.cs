using System;
using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace FloatingText
{
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {
        public override string Author => "FrankV22";
        public override string Description => "Burbujas de Chat cuando los jugadores envian un mensaje.";
        public override string Name => "FloatingText";
        public override Version Version => new Version(1, 1);

        public Plugin(Main game) : base(game) => Order = 0;

        public override void Initialize()
        {
            ServerApi.Hooks.ServerChat.Register(this, OnChat);
            Commands.ChatCommands.Add(new Command("floatingtext.quiet", Quiet, "quiet"));
        }

        private void Quiet(CommandArgs args)
        {
            var color = new Color(args.Player.Group.R, args.Player.Group.G, args.Player.Group.B).PackedValue;
            NetMessage.SendData(119, -1, -1, Terraria.Localization.NetworkText.FromLiteral(string.Join(" ", args.Parameters)), (int)color, args.Player.X + 8, args.Player.Y + 32);
        }

        private void OnChat(ServerChatEventArgs args)
        {
            if (args.Text.StartsWith(Commands.Specifier) || args.Text.StartsWith(Commands.SilentSpecifier))
                return;

            var color = new Color(TShock.Players[args.Who].Group.R, TShock.Players[args.Who].Group.G, TShock.Players[args.Who].Group.B).PackedValue;
            NetMessage.SendData(119, -1, -1, Terraria.Localization.NetworkText.FromLiteral(args.Text), (int)color, TShock.Players[args.Who].X + 8, TShock.Players[args.Who].Y + 32);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                ServerApi.Hooks.ServerChat.Deregister(this, OnChat);

            base.Dispose(disposing);
        }
    }
}