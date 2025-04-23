using ChatbotBackend.Models;

namespace ChatbotBackend.PrimaryAgents
{
    public interface ISessionManager
    {
        Task<CommunicationFormat> TryGetUsersession(string prompt);
    }
}