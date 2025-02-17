namespace MyFinance.Core.Models.Reports;

public record IncomeAndExpenses(string UserId, int Month, int Year, decimal Incomes, decimal Expenses);
