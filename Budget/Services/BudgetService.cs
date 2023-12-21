using System.Runtime.CompilerServices;
using Budget.Repositories;

namespace Budget.Services;

public class BudgetService(IBudgetRepository budgetRepository)
{
    public decimal Query(DateTime start, DateTime end)
    {
        var budgetPeriod = GetBudgetPeriod(start, end);
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

    public Dictionary<DateTime, int> GetBudgetPeriod(DateTime startDate, DateTime endDate)
    {
        var period = new Dictionary<DateTime, int>();
        var currentMonth = startDate;

        while (currentMonth <= endDate)
        {
            var endOfMonth = new DateTime(
                currentMonth.Year, currentMonth.Month, 
                DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month));
            var endOfPeriod = (endOfMonth < endDate) ? endOfMonth : endDate;
            var daysInMonth = (endOfPeriod - currentMonth).Days + 1;
            period.Add(currentMonth, daysInMonth);
            currentMonth = currentMonth.AddMonths(1);
        }
        return period;
    }
}

