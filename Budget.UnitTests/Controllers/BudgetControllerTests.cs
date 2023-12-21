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
                new Models.Budget
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
        _budgetRepository.GetAll().Returns(new List<Models.Budget>
            {
                new Models.Budget
                {
                    YearMonth = "202312",
                    Amount = 3100
                }
            }
        );
        var start = new DateTime(2023, 12, 15);
        var end = new DateTime(2023, 12, 31);
        var actualAmount = _budgetService.Query(start, end);
        actualAmount.Should().Be(1700);
    }
}