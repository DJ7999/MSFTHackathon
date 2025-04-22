using ChatbotBackend.Helper;
using ChatbotBackend.Models;
using ChatbotBackend.Models.MetricModel;
using ChatbotBackend.Models.MetricResponseModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Newtonsoft.Json;
using System.ComponentModel;

namespace ChatbotBackend.Plugins
{
    public class MarketReaserchPlugin
    {
        private readonly Kernel kernel;
        private readonly IChatCompletionService chatCompletionService;
        private readonly TextSearchHelper textSearch;
        private readonly ILogger logger;
        public MarketReaserchPlugin(Kernel kernel, TextSearchHelper textSearch, ILogger<MarketReaserchPlugin> logger)
        {
            this.kernel = kernel.Clone();
            chatCompletionService = this.kernel.GetRequiredService<IChatCompletionService>();

            this.logger = logger;
            this.textSearch = textSearch;
        }



        [KernelFunction]
        [Description($"will give Growth Metrics like Revenue, Profit, Earning Per share, sector Revenue, sector Profit, sector Earning Per share for the provided company name and sector")]
        public async Task<GrowthResponseModel> GrowthMetricPlugin(string companyName, string sector)
        {
            var companyResponse = await textSearch.TextSearchAsync($"what is revenue Growth YOY for company {companyName}") +
                await textSearch.TextSearchAsync($"profit growth YOY for company {companyName}") +
                await textSearch.TextSearchAsync($"what is Earning Per share growth YOY for company {companyName}"); ;
            var sectorResponse = await textSearch.TextSearchAsync($"what is revenue Growth YOY for sector {sector}") +
                await textSearch.TextSearchAsync($"what is profit growth YOY for sector {sector}") +
                await textSearch.TextSearchAsync($"what is Earning Per share growth YoY for sector {sector}"); ;
            logger.LogInformation(companyName, sector);
            PromptExecutionSettings promptExecutionSettings = new AzureOpenAIPromptExecutionSettings
            {

                ResponseFormat = typeof(GrowthResponseModel)
            };

            var response = await chatCompletionService.GetChatMessageContentAsync($"whe have this data for revenue, profit, Earning Per share of last 3 years for company {companyName}" +
                $"and sector {sector} Based on given information  : [{companyResponse}\n{sectorResponse}]" +
                $"return data in structured output format" +
                $"Note all data should be in percentage", promptExecutionSettings);
            var growthResponse = JsonConvert.DeserializeObject<GrowthResponseModel>(response.Content);
            return growthResponse;
        }

        [KernelFunction]
        [Description($"will give Profibility Metrics like ROE, ROCE, Ebitda Margin, NetProfit Margine for the provided company name and sector")]
        public async Task<ProfibilityResponseModel> ProfibilityMetricPlugin(string companyName, string sector)
        {
            var companyResponse = await textSearch.TextSearchAsync($"what is ROE, ROCE, Ebitda Margin, NetProfit Margine for company {companyName}");
            var sectorResponse = await textSearch.TextSearchAsync($"what is ROE, ROCE, Ebitda Margin, NetProfit Margine for sector {sector}");
            logger.LogInformation(companyName, sector);
            PromptExecutionSettings promptExecutionSettings = new AzureOpenAIPromptExecutionSettings
            {

                ResponseFormat = typeof(ProfibilityResponseModel)
            };

            var response = await chatCompletionService.GetChatMessageContentAsync($"we have this data for ROE, ROCE, Ebitda Margin, NetProfit Margine for company {companyName}" +
                $"and sector {sector} Based on given information  : [{companyResponse}\n{sectorResponse}]" +
                $"return data in structured output format", promptExecutionSettings);
            var growthResponse = JsonConvert.DeserializeObject<ProfibilityResponseModel>(response.Content);
            return growthResponse;
        }


        [KernelFunction]
        [Description($"will give valuation Metrics like PEG ratio, PE ratio, pb ratio, Ev Ebidta ratio for the provided company name and sector")]
        public async Task<ValuationResponseModel> ValuationMetricPlugin(string companyName, string sector)
        {
            var companyResponse = await textSearch.TextSearchAsync($"what is PEG ratio, PE ratio, pb ratio, Ev Ebidta ratio for company {companyName}");
            var sectorResponse = await textSearch.TextSearchAsync($"what is PEG ratio, PE ratio, pb ratio, Ev Ebidta ratio for sector {sector}");
            logger.LogInformation(companyName, sector);
            PromptExecutionSettings promptExecutionSettings = new AzureOpenAIPromptExecutionSettings
            {

                ResponseFormat = typeof(ValuationResponseModel)
            };

            var response = await chatCompletionService.GetChatMessageContentAsync($"we have this data for PEG ratio, PE ratio, pb ratio, Ev Ebidta ratio for company {companyName}" +
                $"and sector {sector} Based on given information  : [{companyResponse}\n{sectorResponse}]" +
                $"return data in structured output format", promptExecutionSettings);
            var growthResponse = JsonConvert.DeserializeObject<ValuationResponseModel>(response.Content);
            return growthResponse;
        }

        [KernelFunction]
        [Description($"will give valuation Metrics like PEG ratio, PE ratio, pb ratio, Ev Ebidta ratio for the provided company name and sector")]
        public async Task<LeverageAndSolvencyResponseModel> LeverageAndSolvencyMetricPlugin(string companyName, string sector)
        {
            var companyResponse = await textSearch.TextSearchAsync($"what is Interest Coverage, debt to equity ratio for company {companyName}");
            var sectorResponse = await textSearch.TextSearchAsync($"what is Interest Coverage, debt to equity ratio for sector {sector}");
            logger.LogInformation(companyName, sector);
            PromptExecutionSettings promptExecutionSettings = new AzureOpenAIPromptExecutionSettings
            {

                ResponseFormat = typeof(LeverageAndSolvencyResponseModel)
            };

            var response = await chatCompletionService.GetChatMessageContentAsync($"we have this data for Interest Coverage, debt to equity ratio for company {companyName}" +
                $"and sector {sector} Based on given information  : [{companyResponse}\n{sectorResponse}]" +
                $"return data in structured output format", promptExecutionSettings);
            var growthResponse = JsonConvert.DeserializeObject<LeverageAndSolvencyResponseModel>(response.Content);
            return growthResponse;
        }

        [KernelFunction]
        [Description($"will give Liquidity And Effiiciency Metrics like current ratio, Quick ratio, inventory turnover ratio,Receivables Turnover ratio for the provided company name and sector")]
        public async Task<LiquidityAndEffiiciencyResponseModel> LiquidityAndEffiiciencyMetricPlugin(string companyName, string sector)
        {
            var companyResponse = await textSearch.TextSearchAsync($"what is current ratio, Quick ratio, inventory turnover ratio,Receivables Turnover ratio for company {companyName}");
            var sectorResponse = await textSearch.TextSearchAsync($"what is current ratio, Quick ratio, inventory turnover ratio,Receivables Turnover ratio for sector {sector}");
            logger.LogInformation(companyName, sector);
            PromptExecutionSettings promptExecutionSettings = new AzureOpenAIPromptExecutionSettings
            {

                ResponseFormat = typeof(LiquidityAndEffiiciencyResponseModel)
            };

            var response = await chatCompletionService.GetChatMessageContentAsync($"we have this data for current ratio, Quick ratio, inventory turnover ratio,Receivables Turnover ratio for company {companyName}" +
                $"and sector {sector} Based on given information  : [{companyResponse}\n{sectorResponse}]" +
                $"return data in structured output format", promptExecutionSettings);
            var growthResponse = JsonConvert.DeserializeObject<LiquidityAndEffiiciencyResponseModel>(response.Content);
            return growthResponse;
        }

        [KernelFunction]
        [Description($"will give CashFlow Health Metrics like operating and free cashflow and CashConversionRatio for the provided company name and sector")]
        public async Task<CashFlowHealthResponseModel> CashFlowHealthMetricPlugin(string companyName, string sector)
        {
            var companyResponse = await textSearch.TextSearchAsync($"what is operating cashflow, free cashflow, and Cash Conversion Ratio for company {companyName}");
            logger.LogInformation(companyName, sector);
            PromptExecutionSettings promptExecutionSettings = new AzureOpenAIPromptExecutionSettings
            {

                ResponseFormat = typeof(CashFlowHealthResponseModel)
            };

            var response = await chatCompletionService.GetChatMessageContentAsync($"we have this data for operating cashflow, free cashflow, and Cash Conversion Ratio for company {companyName}" +
                $" Based on given information  : [{companyResponse}]" +
                $"return data in structured output format", promptExecutionSettings);
            var growthResponse = JsonConvert.DeserializeObject<CashFlowHealthResponseModel>(response.Content);
            return growthResponse;
        }

        [KernelFunction]
        [Description($"will give Ownership And Red Flags Metrics like PromoterHolding, PledgedSharesPercent, Fii holding trend any other red flags and free cashflow and CashConversionRatio for the provided company name and sector")]
        public async Task<OwnershipAndRedFlagsResponseModel> OwnershipAndRedFlagsMetricPlugin(string companyName, string sector)
        {
            var companyResponse = await textSearch.TextSearchAsync($"what is Promoter Holding and Pledged Shares Percent for company {companyName}");
            var fiiResponse = await textSearch.TextSearchAsync($"fii holding trend in company {companyName}");
            var redFlagsResponse = await textSearch.TextSearchAsync($"are there any issues company {companyName}");
            logger.LogInformation(companyName, sector);
            PromptExecutionSettings promptExecutionSettings = new AzureOpenAIPromptExecutionSettings
            {

                ResponseFormat = typeof(OwnershipAndRedFlagsResponseModel)
            };

            var response = await chatCompletionService.GetChatMessageContentAsync($"we have this data for Promoter Holding percent and Pledged Shares Percent and " +
                $"fii holding trend and other general issues for company {companyName}" +
                $" Based on given information  : [{companyResponse}]" +
                $"return data in structured output format", promptExecutionSettings);
            var growthResponse = JsonConvert.DeserializeObject<OwnershipAndRedFlagsResponseModel>(response.Content);
            return growthResponse;
        }

        [KernelFunction]
        [Description($"will give verdict for stock on basis of various metric")]
        public Task<VerdictResponseModel> CompanyVerdictPlugin(
[Description("Growth Metric")] GrowthMetricModel growthMetric,
[Description("Profibility Metric")] ProfibilityMetricModel profibilityMetric,
[Description("Valuation Metric")] ValuationMetricModel valuationMetric,
[Description("Leverage And Solvency Metric")] LeverageAndSolvencyMetricModel leverageAndSolvencyMetric,
[Description("Liquidity And Effiiciency Metric")] LiquidityAndEffiiciencyMetricModel liquidityAndEffiiciencyMetric,
[Description("CashFlow Health Metric")] CashFlowHealthMetricModel cashFlowHealthMetric,
[Description("Ownership And Red Flags Metric")] OwnershipAndRedFlagsMetricModel ownership)
        {
            float proFibilityScore = 0;
            if (profibilityMetric.SectorRoe != 0) proFibilityScore += Math.Min(profibilityMetric.Roe / profibilityMetric.SectorRoe, 1) * 35;
            if (profibilityMetric.SectorRoce != 0) proFibilityScore += Math.Min(profibilityMetric.Roce / profibilityMetric.SectorRoce, 1) * 30;
            if (profibilityMetric.SectorNetProfitMargine != 0) proFibilityScore += Math.Min(profibilityMetric.NetProfitMargine / profibilityMetric.SectorNetProfitMargine, 1) * 20;
            if (profibilityMetric.SectorEbitdaMargin != 0) proFibilityScore += Math.Min(profibilityMetric.EbitdaMargin / profibilityMetric.SectorEbitdaMargin, 1) * 15;

            float valuationScore = 0;
            if (valuationMetric.PegRatio != 0) valuationScore += Math.Min(valuationMetric.SectorPegRatio / valuationMetric.PegRatio, 1) * 40;
            if (valuationMetric.PeRatio != 0) valuationScore += Math.Min(valuationMetric.SectorPeRatio / valuationMetric.PeRatio, 1) * 30;
            if (valuationMetric.PbRatio != 0) valuationScore += Math.Min(valuationMetric.SectorPbRatio / valuationMetric.PbRatio, 1) * 15;
            if (valuationMetric.Ev_Ebidta != 0) valuationScore += Math.Min(valuationMetric.SectorEv_Ebidta / valuationMetric.Ev_Ebidta, 1) * 15;

            float leverageAndSolvencyScore = 0;
            if (leverageAndSolvencyMetric.DebtToEquity != 0) leverageAndSolvencyScore += Math.Min(leverageAndSolvencyMetric.SectorDebtToEquity / leverageAndSolvencyMetric.DebtToEquity, 1) * 50;
            if (leverageAndSolvencyMetric.SectorDebtToEquity != 0) leverageAndSolvencyScore += Math.Min(leverageAndSolvencyMetric.InterestCoverage / leverageAndSolvencyMetric.SectorInterestCoverage, 1) * 50;

            float liquidityAndEffiiciencyScore = 0;
            if (liquidityAndEffiiciencyMetric.SectorCurrentRatio != 0) liquidityAndEffiiciencyScore += Math.Min(liquidityAndEffiiciencyMetric.CurrentRatio / liquidityAndEffiiciencyMetric.SectorCurrentRatio, 1) * 25;
            if (liquidityAndEffiiciencyMetric.SectorQuickRatio != 0) liquidityAndEffiiciencyScore += Math.Min(liquidityAndEffiiciencyMetric.QuickRatio / liquidityAndEffiiciencyMetric.SectorQuickRatio, 1) * 25;
            if (liquidityAndEffiiciencyMetric.SectorInventoryTurnover != 0) liquidityAndEffiiciencyScore += Math.Min(liquidityAndEffiiciencyMetric.InventoryTurnover / liquidityAndEffiiciencyMetric.SectorInventoryTurnover, 1) * 25;
            if (liquidityAndEffiiciencyMetric.SectorReceivablesTurnover != 0) liquidityAndEffiiciencyScore += Math.Min(liquidityAndEffiiciencyMetric.ReceivablesTurnover / liquidityAndEffiiciencyMetric.ReceivablesTurnover, 1) * 25;

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
            if (growthMetric.SectorRevenue != 0) valuationScore += Math.Min(growthMetric.Revenue / growthMetric.SectorRevenue, 1) * 35;
            if (growthMetric.SectorProfit != 0) valuationScore += Math.Min(growthMetric.Profit / growthMetric.SectorProfit, 1) * 35;
            if (growthMetric.SectorEarningPerShareCagr != 0) valuationScore += Math.Min(growthMetric.EarningPerShareCagr / growthMetric.SectorEarningPerShareCagr, 1) * 30;
            var response = new VerdictResponseModel();
            var finalScore = (0.25 * proFibilityScore)
                            + (0.2 * growthScore)
                            + (0.15 * valuationScore)
                            + (0.15 * leverageAndSolvencyScore)
                            + (0.1 * cashFlowHealthScore)
                            + (0.1 * liquidityAndEffiiciencyScore)
                            + (0.05 * ownershipAndRedflagScore);
            if (finalScore > 85) response.StockVerdict = StockVerdict.StrongBuy;
            else if (finalScore >= 75) response.StockVerdict = StockVerdict.Good;
            else if (finalScore >= 60) response.StockVerdict = StockVerdict.Watchlist;
            else if (finalScore >= 50) response.StockVerdict = StockVerdict.Weak;
            else response.StockVerdict = StockVerdict.Exit;
            response.score = (float)finalScore;
            logger.LogInformation(response.StockVerdict + " " + response.score);
            return Task.FromResult(response);
        }

    }
}
