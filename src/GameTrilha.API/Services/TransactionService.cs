using GameTrilha.API.Services.Interfaces;
using GameTrilha.API.Contexts;
using GameTrilha.API.ViewModels.UserViewModels;
using GameTrilha.Domain.Entities;

namespace GameTrilha.API.Services;

public class TransactionService : ITransactionService
{
    private readonly TrilhaContext _trilhaContext;

    public TransactionService(TrilhaContext context)
    {
        _trilhaContext = context;
    }

    public bool RegisterOrder(OrderDetailsViewModel userOrder)
    {
        User? userData = SearchUserByEmail(userOrder.Mail);

        if (userData is null)
        {
            return false;
        }

        UpdateUserBalance(userData, userOrder.Coins);
        SaveUpdate();

        return true;
    }

    private User? SearchUserById(string userId)
    {
        User? userData = _trilhaContext.Users.Find(userId);

        return userData;
    }

    private User? SearchUserByEmail(string userMail)
    {
        User? userData = _trilhaContext.Users.SingleOrDefault(user => user.Email == userMail);
        
        return userData;
    }

    private void UpdateUserBalance(User userData, string coins)
    {
        int coinsValue = int.Parse(coins);

        userData.AddBalance(coinsValue);
    }

    private void SaveUpdate()
    {
        _trilhaContext.SaveChanges();
    }
}