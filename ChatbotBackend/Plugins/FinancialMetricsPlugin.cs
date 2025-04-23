using ChatbotBackend.Models;
using ChatbotBackend.Models.MetricModel;
using ChatbotBackend.Models.MetricResponseModel;
using ChatbotBackend.Services;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using System.ComponentModel;
using System.Text.Json;


namespace ChatbotBackend.Plugins
{
    public class FinancialMetricsPlugin
    {
        private readonly TextSearchHelper textSearch;
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatCompletionService;
        private readonly ILogger logger;
        public FinancialMetricsPlugin(Kernel kernel, TextSearchHelper textSearch, ILogger<FinancialMetricsPlugin> logger)
        {
            _kernel = kernel.Clone();
            this.textSearch = textSearch;
            _chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            this.logger = logger;
        }

        [KernelFunction]
        [Description($"will give Growth Metrics like Revenue, Profit, Earning Per share, sector Revenue, sector Profit, sector Earning Per share for the provided company name and sector")]
        public async Task<GrowthResponseModel> GrowthMetricPlugin(string companyName, string sector)
        {
            var companyResponse = await textSearch.TextSearchAsync($"what is revenue Cagr for company {companyName}") +
                await textSearch.TextSearchAsync($"what is profit Cagr for company {companyName}") +
                await textSearch.TextSearchAsync($"what is Earning Per share Cagr for company {companyName}"); ;
            var sectorResponse = await textSearch.TextSearchAsync($"what is revenue Cagr, profit Cagr, Earning Per share Cagr for sector {sector}");
            logger.LogInformation(companyName, sector);

            var response = await _chatCompletionService.GetChatMessageContentAsync($"whe have this data for revenue, profit, Earning Per share of last 3 years for company {companyName}" +
                $"and sector {sector} Based on given information  : [{companyResponse}\n{sectorResponse}]" +
                $"return data in structured output format", new AzureOpenAIPromptExecutionSettings { ResponseFormat = typeof(GrowthResponseModel) });
            var growthResponse = JsonSerializer.Deserialize<GrowthResponseModel>(response.Content);
            return growthResponse;
        }


        [KernelFunction]
        [Description($"will give verdict on basis of various metric")]
        public Task<StockVerdict> CompanyVerdictPlugin(
        [Description("Growth Metric")] GrowthMetricModel growthMetric,
        [Description("Profibility Metric")] ProfibilityMetricModel profibilityMetric,
        [Description("Valuation Metric")] ValuationMetricModel valuationMetric,
        [Description("Leverage And Solvency Metric")] LeverageAndSolvencyMetricModel leverageAndSolvencyMetric,
        [Description("Liquidity And Effiiciency Metric")] LiquidityAndEffiiciencyMetricModel liquidityAndEffiiciencyMetric,
        [Description("CashFlow Health Metric")] CashFlowHealthMetricModel cashFlowHealthMetric,
        [Description("Ownership And Red Flags Metric")] OwnershipAndRedFlagsMetricModel ownership)
        {
            float proFibilityScore = 0;
            proFibilityScore += Math.Min(profibilityMetric.Roe / profibilityMetric.SectorRoe, 1) * 35;
            proFibilityScore += Math.Min(profibilityMetric.Roce / profibilityMetric.Roce, 1) * 30;
            proFibilityScore += Math.Min(profibilityMetric.NetProfitMargine / profibilityMetric.SectorNetProfitMargine, 1) * 20;
            proFibilityScore += Math.Min(profibilityMetric.EbitdaMargin / profibilityMetric.SectorEbitdaMargin, 1) * 15;

            float valuationScore = 0;
            valuationScore += Math.Min(valuationMetric.SectorPegRatio / valuationMetric.PegRatio, 1) * 40;
            valuationScore += Math.Min(valuationMetric.SectorPeRatio / valuationMetric.PeRatio, 1) * 30;
            valuationScore += Math.Min(valuationMetric.SectorPbRatio / valuationMetric.PbRatio, 1) * 15;
            valuationScore += Math.Min(valuationMetric.SectorEv_Ebidta / valuationMetric.Ev_Ebidta, 1) * 15;

            float leverageAndSolvencyScore = 0;
            leverageAndSolvencyScore += Math.Min(leverageAndSolvencyMetric.SectorDebtToEquity / leverageAndSolvencyMetric.DebtToEquity, 1) * 50;
            leverageAndSolvencyScore += Math.Min(leverageAndSolvencyMetric.InterestCoverage / leverageAndSolvencyMetric.SectorInterestCoverage, 1) * 50;

            float liquidityAndEffiiciencyScore = 0;
            liquidityAndEffiiciencyScore += Math.Min(liquidityAndEffiiciencyMetric.CurrentRatio / liquidityAndEffiiciencyMetric.SectorCurrentRatio, 1) * 25;
            liquidityAndEffiiciencyScore += Math.Min(liquidityAndEffiiciencyMetric.QuickRatio / liquidityAndEffiiciencyMetric.SectorQuickRatio, 1) * 25;
            liquidityAndEffiiciencyScore += Math.Min(liquidityAndEffiiciencyMetric.InventoryTurnover / liquidityAndEffiiciencyMetric.SectorInventoryTurnover, 1) * 25;
            liquidityAndEffiiciencyScore += Math.Min(liquidityAndEffiiciencyMetric.ReceivablesTurnover / liquidityAndEffiiciencyMetric.ReceivablesTurnover, 1) * 25;

            float cashFlowHealthScore = 0;
            cashFlowHealthScore += (cashFlowHealthMetric.OperatingCashFlow > 0 ? 1 : 0) * 30;
            cashFlowHealthScore += (cashFlowHealthMetric.FreeCashFlow > 0 ? 1 : 0) * 30;
            cashFlowHealthScore += Math.Min(cashFlowHealthMetric.CashConversionRatio, 1) * 40;

            float ownershipAndRedflagScore = 0;
            ownershipAndRedflagScore += Math.Min(ownership.PromoterHolding / 50, 1) * 30;
            ownershipAndRedflagScore += (ownership.PledgedSharesPercent <= 0 ? 1 : 0) * 30;
            ownershipAndRedflagScore += (ownership.IsFiiDiiHoldingIncreasing ? 1 : 0) * 20;
            ownershipAndRedflagScore += (ownership.HasRedFlags ? 0 : 1) * 20;

            float growthScore = 0;
            //calclate here
            var finalScore = (0.25 * proFibilityScore)
                            + (0.2 * growthScore)
                            + (0.15 * valuationScore)
                            + (0.15 * leverageAndSolvencyScore)
                            + (0.1 * cashFlowHealthScore)
                            + (0.1 * liquidityAndEffiiciencyScore)
                            + (0.05 * ownershipAndRedflagScore);
            if (finalScore > 85) return Task.FromResult(StockVerdict.StrongBuy);
            if (finalScore >= 75) return Task.FromResult(StockVerdict.Good);
            if (finalScore >= 60) return Task.FromResult(StockVerdict.Watchlist);
            if (finalScore >= 50) return Task.FromResult(StockVerdict.Weak);
            return Task.FromResult(StockVerdict.Exit);
        }
    }
}
