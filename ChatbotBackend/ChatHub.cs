﻿using ChatbotBackend.Models;
using ChatbotBackend.PrimaryAgents;
using ChatbotBackend.Services;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using static ChatbotBackend.PrimaryAgents.AgentRouter;

namespace ChatbotBackend
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, IServiceScope> _scopes = new();
        private static readonly ConcurrentDictionary<string, IAgentRouter> _router = new();
        private static readonly ConcurrentDictionary<string, ISessionManager> _session = new();
        public static readonly ConcurrentDictionary<string, int> _connectionUserMap = new();
        public static HubCallerContext sessionContext { get; private set; }

        private IAgentRouter _agentRouter;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMarkDownGenerator _markDownGenerator;
        private ISessionManager _sessionManager;
        UserContext _userContext;

        public ChatHub(IAgentRouter agentRouter, IServiceScopeFactory scopeFactory, IMarkDownGenerator markDownGenerator)
        {
            _scopeFactory = scopeFactory;
            _markDownGenerator = markDownGenerator;

            
            _agentRouter = agentRouter;


        }

        public override Task OnConnectedAsync()
        {
            var scope = _scopeFactory.CreateScope();
            _scopes[Context.ConnectionId] = scope;


            // Optionally resolve an agent now, or delay until needed
            // If each user should get their own agent immediately:
            var router = scope.ServiceProvider.GetRequiredService<IAgentRouter>();
            _router[Context.ConnectionId] = router;
            _session[Context.ConnectionId] = scope.ServiceProvider.GetRequiredService<ISessionManager>(); ;

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {

            if (_scopes.TryRemove(Context.ConnectionId, out var scope))
            {
                scope.Dispose();
            }

            _router.TryRemove(Context.ConnectionId, out _);
            _session.TryRemove(Context.ConnectionId, out _);
            _connectionUserMap.TryRemove(Context.ConnectionId, out _);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CommunicationFormat input)
        {
            _userContext = _scopes[Context.ConnectionId].ServiceProvider.GetRequiredService<UserContext>();
            _userContext.Id = Context.ConnectionId;
            if (!_connectionUserMap.TryGetValue(Context.ConnectionId, out _))
            {
                if (_session.TryGetValue(Context.ConnectionId, out var session))
                {
                    _sessionManager = session;
                }
                var response = await _sessionManager.TryGetUsersession(input.Message);
                await Clients.Caller.SendAsync("ReceiveMessage", response.User, response.Message);
                return;
            }

            if (_router.TryGetValue(Context.ConnectionId, out var router))
            {
                _agentRouter = router;
            }

            RouterResponse routerResponse = await _agentRouter.GetAgentAsync(input.Message);
            if (!string.IsNullOrWhiteSpace(routerResponse.UserMessage))
                await Clients.Caller.SendAsync("ReceiveMessage", "Assistant", routerResponse.UserMessage);

            if (routerResponse.Agents == null)
            {
                return;
            }
            _router[Context.ConnectionId] = _agentRouter; // cache it now

            foreach (var agent in routerResponse.Agents)
            {
                CommunicationFormat output = await agent.Agent.GetAgentResponseAsync(agent.Prompt);
                _agentRouter.UpdateRouterMemory(output.Message, output.User);
                output.Message = await _markDownGenerator.GenerateMarkdown(output.Message);
                await Clients.Caller.SendAsync("ReceiveMessage", output.User, output.Message);
            }
        }
    }
}
