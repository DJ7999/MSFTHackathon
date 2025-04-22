namespace ChatbotBackend.Manager
{
    public interface ISessionManager
    {
        Task<CommunicationFormat> TryGetUsersession(string prompt);
    }
}