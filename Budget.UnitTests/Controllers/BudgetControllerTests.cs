using Budget.Repositories;
using Budget.Services;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework.Internal;

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
    public void single_month_and_round_to_2nd_digit()
    {
        _budgetRepository.GetAll().Returns(new List<Models.Budget>
            {
                new()
                {
                    YearMonth = "202312",
                    Amount = 3000
                }
            }
        );
        var start = new DateTime(2023, 12, 1);
        var end = new DateTime(2023, 12, 10);
        var actualAmount = _budgetService.Query(start, end);
        actualAmount.Should().Be(967.74m);
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

    [Test]
    public void cross_three_months()
    {
        GivenBudgets(new List<Models.Budget>
        {
            new()
            {
                YearMonth = "202310",
                Amount = 31
            },
            new()
            {
                YearMonth = "202311",
                Amount = 3000
            },
            new()
            {
                YearMonth = "202312",
                Amount = 31000
            },
            
        });
        var actualAmount = WhenQuery(
            new DateTime(2023, 10, 10), 
            new DateTime(2023, 12, 12));
        actualAmount.Should().Be(22 + 3000 + 12000);
    }

    [Test]
    public void illegal_start_end()
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
            new DateTime(2023, 12, 31),
            new DateTime(2023, 12, 15)); 
        actualAmount.Should().Be(0);
    }

    [Test]
    public void lack_of_month()
    {
        GivenBudgets(new List<Models.Budget>
        {
            new()
            {
                YearMonth = "202310",
                Amount = 31
            },
            new()
            {
                YearMonth = "202312",
                Amount = 31000
            },
        });
        var actualAmount = WhenQuery(
            new DateTime(2023, 10, 10), 
            new DateTime(2023, 12, 12));
        actualAmount.Should().Be(22 + 12000);
    }

    private decimal WhenQuery(DateTime start, DateTime end)
    {
        return _budgetService.Query(start, end);
    }

    private void GivenBudgets(List<Models.Budget> budgets)
    {
        _budgetRepository.GetAll().Returns(budgets);
    }
}