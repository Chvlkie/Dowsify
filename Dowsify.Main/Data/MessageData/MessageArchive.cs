﻿namespace Dowsify.Main.Data
{
    [Serializable]
    public class MessageArchive : IEquatable<MessageArchive?>
    {
        public int MessageId { get; set; }
        public string MessageText { get; set; }

        public MessageArchive(int messageId, string messageText)
        {
            MessageId = messageId;
            MessageText = messageText;
        }

        public override bool Equals(object? obj) => Equals(obj as MessageArchive);

        public bool Equals(MessageArchive? other) => other is not null &&
                   MessageId == other.MessageId &&
                   MessageText == other.MessageText;

        public override int GetHashCode() => HashCode.Combine(MessageId, MessageText);
    }
}