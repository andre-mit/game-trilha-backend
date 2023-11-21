namespace GameTrilha.API.ViewModels.UserViewModels;

public class OrderDetailsViewModel
{
    public string Mail { get; set; }
    public string Price { get; set; }
    public string Coins { get; set; }

    public OrderDetailsViewModel(string mail, string price, string coins)
    {
        Mail = mail;
        Price = price;
        Coins = coins;
    }
}