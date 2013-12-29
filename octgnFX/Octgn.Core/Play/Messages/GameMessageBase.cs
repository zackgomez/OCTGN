namespace Octgn.Core.Play.Messages
{
    using GalaSoft.MvvmLight.Messaging;

    public abstract class GameMessageBase
    {
        public bool IsMutable { get; private set; }

        public string RawMessage { get; private set; }

        protected GameMessageBase(bool isMutable)
        {
            IsMutable = isMutable;
            RawMessage = FormatMessage();
        }

        public abstract string FormatMessage();

        public void Send()
        {
            Messenger.Default.Send(this);
        }
    }
}