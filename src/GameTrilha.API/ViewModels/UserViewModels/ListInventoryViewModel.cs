using GameTrilha.API.ViewModels.BoardViewModels;
using GameTrilha.API.ViewModels.SkinViewModels;
using GameTrilha.Domain.Entities;

namespace GameTrilha.API.ViewModels.UserViewModels
{
    public class ListInventoryViewModel
    {
        public List<ListSkinViewModel> Skins { get; set; }
        public List<ListBoardViewModel> Boards { get; set; }

        public ListInventoryViewModel(List<ListBoardViewModel> board, List<ListSkinViewModel> skin)
        {
            Skins = skin;
            Boards = board;
        }

    }
}
