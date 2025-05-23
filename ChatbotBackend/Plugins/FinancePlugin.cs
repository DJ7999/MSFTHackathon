﻿using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace ChatbotBackend.Plugins
{
    public class FinancePlugin
    {
        [KernelFunction]
        [Description("Calculates the required monthly investment to reach a financial goal.")]
        public Task<double> CalculateMonthlyInvestmentAsync(
        [Description("Target amount to achieve.")] double futureValue,
        [Description("Investment duration in years.")] int investmentDurationYears,
        [Description("Annual return rate in percentage (default is 7 for 7%).")] double annualReturnRate = 7,
        [Description("Initial investment amount (default is 0.")] double initialInvestment = 0)
        {
            double monthlyRate = annualReturnRate / 12 / 100;
            int totalMonths = investmentDurationYears * 12;

            double futureValueOfInitialInvestment = initialInvestment * Math.Pow(1 + monthlyRate, totalMonths);
            double requiredFutureValueFromPMT = futureValue - futureValueOfInitialInvestment;

            double monthlyInvestment;
            if (monthlyRate == 0)
            {
                monthlyInvestment = requiredFutureValueFromPMT / totalMonths;
                return Task.FromResult(Math.Round(monthlyInvestment, 2));
            }

            double denominator = (Math.Pow(1 + monthlyRate, totalMonths) - 1) / monthlyRate;
            monthlyInvestment = requiredFutureValueFromPMT / denominator;

            return Task.FromResult(Math.Round(monthlyInvestment, 2));
        }

        [KernelFunction]
        [Description("Calculates the future value of a Systematic Investment Plan (SIP).")]
        public Task<double> CalculateSipFutureValueAsync(
        [Description("Monthly investment amount.")] double monthlyInvestment,
        [Description("Expected annual rate of return (in percentage).")] double annualReturnRate,
        [Description("Investment duration in years.")] int investmentDurationYears)
        {
            double monthlyRate = annualReturnRate / 12 / 100;
            int totalMonths = investmentDurationYears * 12;
            double futureValue = monthlyInvestment * (Math.Pow(1 + monthlyRate, totalMonths) - 1) * (1 + monthlyRate) / monthlyRate;
            return Task.FromResult(Math.Round(futureValue, 2));
        }

        [KernelFunction]
        [Description("Calculates the future value of today's amount considering inflation.")]
        public Task<double> AdjustForInflationAsync(
            [Description("The amount of money you have today.")] double presentAmount,
            [Description("Number of years into the future.")] int years,
            [Description("Yearly inflation rate in percent (default is 6%).")] double inflationRate = 6)
        {
            double futureValue = presentAmount * Math.Pow(1 + inflationRate / 100, years);
            return Task.FromResult(Math.Round(futureValue, 2));
        }
    }
}
