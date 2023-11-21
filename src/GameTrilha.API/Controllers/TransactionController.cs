using GameTrilha.API.Services.Interfaces;
using GameTrilha.API.Services;
using GameTrilha.API.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace GameTrilha.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost("RegisterPurchase")]
    public IActionResult RegisterPurchase(OrderDetailsViewModel userOrder)
    {
        try
        {
            _transactionService.RegisterOrder(userOrder);
            return Ok();
        }
        catch (Exception exception)
        {
            return BadRequest("An error occurred while registering the user purchase!");
        }
    }
}