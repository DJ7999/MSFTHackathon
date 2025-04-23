using ChatbotBackend.Models;

namespace ChatbotBackend.Agent
{
    public interface IAgent
    {
        Task<CommunicationFormat> GetAgentResponseAsync(string message);
    }
}
