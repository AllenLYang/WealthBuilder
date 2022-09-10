using Microsoft.AspNetCore.Components;

namespace WealthBuilder.Pages
{
    public partial class Confirmation
    {
        private bool DisplayConfirmation = false;

        [Parameter]
        public string Title { get; set; } = "Input Error";

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public EventCallback onConfirm { get; set; }

        [Parameter]
        public EventCallback onCancel { get; set; }

        public void Show() => DisplayConfirmation = true;
        public void Hide() => DisplayConfirmation = false;
    }
}
