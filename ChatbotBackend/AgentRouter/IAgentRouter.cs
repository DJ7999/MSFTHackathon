using ChatbotBackend.Agent;

namespace ChatbotBackend.AgentRouter
{
    public interface IAgentRouter
    {
        Task<(IAgent? Agent, string ResponseMessage)> GetAgentAsync(string message);

        void UpdateRouterMemory(string message, string author);
    }
}
