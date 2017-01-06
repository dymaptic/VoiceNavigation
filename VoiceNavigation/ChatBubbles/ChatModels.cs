using System;
namespace VoiceNavigation
{
	public enum ChatMessageType
	{
		Incoming,
		Outgoing,
	}

	public class ChatMessage
	{
		public ChatMessageType Type { get; set; }
		public string Text { get; set; }
	}
}
