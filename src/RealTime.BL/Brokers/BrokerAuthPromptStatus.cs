
namespace RealTime.BL.Brokers
{
    public enum BrokerAuthPromptStatus
    {
        /// <summary>
        /// Set when the request to get auth prompt ended up with an error
        /// </summary>
        Failed,
        /// <summary>
        /// Indicates that the client should redirect to the provided link
        /// </summary>
        Redirect,
        /// <summary>
        /// Indicates that the client should open provided broker module for authentication
        /// </summary>
        OpenInBrokerModule
    }
}
