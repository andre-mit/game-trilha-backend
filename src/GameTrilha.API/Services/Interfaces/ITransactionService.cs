using GameTrilha.API.ViewModels.UserViewModels;

namespace GameTrilha.API.Services.Interfaces;

public interface ITransactionService
{
    bool RegisterOrder(OrderDetailsViewModel userOrder);
}