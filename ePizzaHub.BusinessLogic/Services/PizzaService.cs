using ePizzaHub.BusinessLogic.Interfaces;
using ePizzaHub.Models;

namespace ePizzaHub.BusinessLogic.Services
{
    public class PizzaService
    {
        private readonly IPizzaRepository _pizzaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PizzaService(IPizzaRepository pizzaRepository, IUnitOfWork unitOfWork)
        {
            _pizzaRepository = pizzaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PizzaModel>> GetPizzaAsync()
        {
            return await _pizzaRepository.Get();
        }

        public async Task<PizzaModel?> GetPizzaByUidAsync(Guid uid)
        {
            return await _pizzaRepository.Find(uid);
        }

        public async Task<GridDataModel<PizzaModel>> GetPizzaExtendedForGridAsync(FilterModel filter)
        {
            return await _pizzaRepository.GetExtendedForGrid(filter);
        }

        public async Task<PizzaModel> CreatePizzaAsync(PizzaModel model)
        {
            try
            {
                await _unitOfWork.BeginAsync();
                var result = await _pizzaRepository.Add(model);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<PizzaModel> UpdatePizzaAsync(PizzaModel model)
        {
            try
            {
                await _unitOfWork.BeginAsync();
                var result = await _pizzaRepository.Update(model);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<int> DeletePizzaAsync(Guid uid)
        {
            try
            {
                await _unitOfWork.BeginAsync();

                var targetPizza = await _pizzaRepository.Find(uid);
                if (targetPizza == null)
                {
                    await _unitOfWork.RollbackAsync();
                    return 0;
                }

                await _pizzaRepository.Remove(targetPizza);
                var changes = await _unitOfWork.CompleteAsync();

                await _unitOfWork.CommitAsync();
                return changes;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}