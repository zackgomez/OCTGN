namespace Octgn.Play.Messages
{
    using System.Collections.Generic;

    using Octgn.Core.Play.Messages;

    public abstract class GameMessage : GameMessageBase
    {
        public bool IsMuted { get; private set; }

        protected GameMessage(bool isMutable): base(isMutable)
        {
            IsMuted = Program.Client.Muted != 0;
        }

        public string MergeArgs(string format, IEnumerable<object> args, int startAt = 0)
        {
            var i = 0;
            foreach (var a in args)
            {
                try
                {
                    var placeholder = "{" + i + "}";
                    var arg = a;
                    if (arg is Card)
                    {
                        var card = arg as Card;
                        if (card.FaceUp || card.MayBeConsideredFaceUp)
                            arg = card.Type;
                    }
                    if (arg is DataNew.Entities.Card || arg is CardIdentity)
                    {
                        var card = arg as DataNew.Entities.Card;
                        var cardIdentity = arg as CardIdentity;
                        if (card == null) card = cardIdentity.Model;
                        format = format.Replace(placeholder, "{" + card.Id + "}");
                        continue;
                    }
                    format = format.Replace(placeholder, arg == null ? "[?]" : arg.ToString());
                }
                finally
                {
                    i++;
                }
            }
            return format;
        }

        public static WarningGameMessage Warning(string message, params object[] args)
        {
            return new WarningGameMessage(message, args);
        }

        public static ChatGameMessage Chat(Player from, string message)
        {
            return new ChatGameMessage(from, message);
        }

        public static PlayerEventGameMessage PlayerEvent(Player player, string message, params object[] args)
        {
            return new PlayerEventGameMessage(player, message, args);
        }
    }


    public abstract class FormattedGameMessage : GameMessage
    {
        private readonly string message;
        private readonly object[] args;

        protected FormattedGameMessage(bool isMutable,string message, params object[] args)
            : base(isMutable)
        {
            this.message = message;
            this.args = args;
        }

        public override string FormatMessage()
        {
            return this.MergeArgs(message, args);
        }
    }

    public class ChatGameMessage : GameMessage
    {
        public Player From { get; set; }
        public string Message { get; set; }

        public ChatGameMessage(Player from, string message)
            : base(false)
        {
            From = from;
            Message = message;
        }

        public override string FormatMessage()
        {
            return string.Format("<{0}> {1}", From, Message);
        }
    }

    public class WarningGameMessage : FormattedGameMessage
    {
        public WarningGameMessage(string message, params object[] args)
            : base(false,message, args)
        {
        }
    }

    public class ErrorGameMessage : FormattedGameMessage
    {
        public ErrorGameMessage(string message, params object[] args)
            : base(false,message, args)
        {
        }
    }

    public class PlayerEventGameMessage : FormattedGameMessage
    {
        public Player Player { get; private set; }

        public PlayerEventGameMessage(Player player,string message, params object[] args)
            : base(true,message, args)
        {
            this.Player = player;
        }

        public override string FormatMessage()
        {
            return Player.ToString() + " " + base.FormatMessage();
        }
    }
}