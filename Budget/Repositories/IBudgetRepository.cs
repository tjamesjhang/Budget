namespace Budget.Repositories;

public interface IBudgetRepository
{
    List<Models.Budget> GetAll();
}