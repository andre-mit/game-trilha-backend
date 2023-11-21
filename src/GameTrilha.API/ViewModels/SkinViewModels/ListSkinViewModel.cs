namespace GameTrilha.API.ViewModels.SkinViewModels
{
    public class ListSkinViewModel
    {
        public ListSkinViewModel(Guid id, string name, string src, string? description, double price, bool selected = false)
        {
            Id = id;
            Name = name;
            Src = src;
            Description = description;
            Price = price;
            Selected = selected;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Src { get; set; }
        public string? Description { get; set; }
        public double Price { get; set; }
        public bool Selected { get; set; }
    }
}
