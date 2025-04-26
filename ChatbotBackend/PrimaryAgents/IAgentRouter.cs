using static ChatbotBackend.PrimaryAgents.AgentRouter;

namespace ChatbotBackend.PrimaryAgents
{
    public interface IAgentRouter
    {
        Task<RouterResponse> GetAgentAsync(string message);

        void UpdateRouterMemory(string message, string author);
    }
}
