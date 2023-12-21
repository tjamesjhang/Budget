using System.Runtime.CompilerServices;
using Budget.Models;
using Budget.Repositories;

namespace Budget.Services;

public class BudgetService(IBudgetRepository budgetRepository)
{
    public decimal Query(DateTime start, DateTime end)
    {
        if (start > end)
        {
            return 0;
        }
        
        var daysInMonth = new BudgetDateTime(start, end).GetDaysInMonth();
        var budgets = budgetRepository.GetAll();
        
        var originalBudget = budgets
            .Join(
                daysInMonth, 
                budget => budget.YearMonth, 
                period => period.Key.ToString("yyyyMM"),
                (budget, period) =>
                {
                    var singleDayAmount = (decimal) budget.Amount / DateTime.DaysInMonth(period.Key.Year, period.Key.Month);
                    return singleDayAmount * period.Value;
                })
            .Sum(monthAmount => monthAmount);
        return decimal.Round(originalBudget, 2);
    }
}