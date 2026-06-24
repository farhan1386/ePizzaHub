using ePizzaHub.Models;

namespace ePizzaHub.BusinessLogic.Interfaces
{
    public interface IPizzaRepository
    {
        Task<IEnumerable<PizzaModel>> Get();
        Task<PizzaModel?> Find(Guid uid);
        Task<GridDataModel<PizzaModel>> GetExtendedForGrid(FilterModel filter);
        Task<PizzaModel> Add(PizzaModel model);
        Task<PizzaModel> Update(PizzaModel model);
        Task<int> Remove(PizzaModel model);
    }
}
