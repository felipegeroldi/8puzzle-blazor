using Microsoft.AspNetCore.Components;

namespace EightPuzzle.Components
{
    public partial class Card : ComponentBase
    {
        [Parameter]
        public string Title { get; set; }
        [Parameter]
        public RenderFragment ChildContent { get; set; }
    }
}
