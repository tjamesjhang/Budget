namespace Budget.Models;

public class BudgetDateTime(DateTime start, DateTime end)
{
    public DateTime Start { get; } = start;
    public DateTime End { get; } = end;

    public Dictionary<DateTime, int> GetBudgetPeriod()
    {
        var period = new Dictionary<DateTime, int>();
        var currentMonth = Start;

        while (currentMonth <= End)
        {
            var endOfMonth = new DateTime(
                currentMonth.Year, currentMonth.Month, 
                DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month));
            var endOfPeriod = (endOfMonth < End) ? endOfMonth : End;
            var daysInMonth = (endOfPeriod - currentMonth).Days + 1;
            period.Add(currentMonth, daysInMonth);
            var nextMonth = currentMonth.AddMonths(1);
            currentMonth = new DateTime(nextMonth.Year, nextMonth.Month, 1); 
        }
        return period;
    }
}