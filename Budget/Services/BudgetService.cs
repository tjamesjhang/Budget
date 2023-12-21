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
        
        var budgetPeriod = new BudgetDateTime(start, end).GetBudgetPeriod();
        var budgets = budgetRepository.GetAll();
        
        return budgets
            .Join(
                budgetPeriod, 
                budget => budget.YearMonth, 
                period => period.Key.ToString("yyyyMM"),
                (budget, period) =>
                {
                    var singleDayAmount = budget.Amount / DateTime.DaysInMonth(period.Key.Year, period.Key.Month);
                    return singleDayAmount * period.Value;
                })
            .Sum(monthAmount => monthAmount);
    }
}