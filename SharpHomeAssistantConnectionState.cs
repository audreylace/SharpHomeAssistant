namespace AudreysCloud.Community.SharpHomeAssistant
{
	/// <summary>
	/// Enum representing the state of the sharp home assistant connection.
	/// </summary>
	public enum SharpHomeAssistantConnectionState
	{
		/// <summary>
		/// No connection is open to the remote home assistant instance.
		/// </summary>
		NotConnected,
		/// <summary>
		/// A websocket connection is open and the instance is in the process of 
		/// authenticating.
		/// </summary>
		Connecting,
		/// <summary>
		/// The connection is open and messages can not be sent and received over it.
		/// </summary>
		Connected,
		/// <summary>
		/// The connection is in the closing state. A close message has been either sent or received over
		/// the websocket connection. The instance is unwiding the connection and flushing all remaining messages.
		/// </summary>
		Closing,
		/// <summary>
		/// The instance closed the connection uncleanly. The instance can not be re-used in this state since
		/// aborting leaves various internal resources in ill defined states.
		/// </summary>
		Aborted
	}
}