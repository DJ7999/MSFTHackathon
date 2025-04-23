using ChatbotBackend.Agent;

namespace ChatbotBackend.PrimaryAgents
{
    public interface IAgentRouter
    {
        Task<(IAgent? Agent, string ResponseMessage)> GetAgentAsync(string message);

        void UpdateRouterMemory(string message, string author);
    }
}
