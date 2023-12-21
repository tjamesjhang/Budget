using Budget.Repositories;
using Budget.Services;
using FluentAssertions;
using NSubstitute;

namespace Budget.UnitTests.Controllers;

public class BudgetControllerTests
{
    private IBudgetRepository _budgetRepository;
    private BudgetService _budgetService;

    [SetUp]
    public void SetUp()
    {
        _budgetRepository = Substitute.For<IBudgetRepository>();
        _budgetService = new BudgetService(_budgetRepository);
    }

    [Test]
    public void single_month()
    {
        _budgetRepository.GetAll().Returns(new List<Models.Budget>
            {
                new()
                {
                    YearMonth = "202312",
                    Amount = 3100
                }
            }
        );
        var start = new DateTime(2023, 12, 1);
        var end = new DateTime(2023, 12, 31);
        var actualAmount = _budgetService.Query(start, end);
        actualAmount.Should().Be(3100);
    }
    
    [Test]
    public void partial_month()
    {
        GivenBudgets(new List<Models.Budget>
        {
            new()
            {
                YearMonth = "202312",
                Amount = 3100
            }
        });
        var actualAmount = WhenQuery(
            new DateTime(2023, 12, 15), 
            new DateTime(2023, 12, 31));
        actualAmount.Should().Be(1700);
    }

    private decimal WhenQuery(DateTime start, DateTime end)
    {
        var actualAmount = _budgetService.Query(start, end);
        return actualAmount;
    }

    private void GivenBudgets(List<Models.Budget> budgets)
    {
        _budgetRepository.GetAll().Returns(budgets);
    }
}