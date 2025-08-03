using JotLink.Pages;

namespace JotLink
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(Notes_List),typeof(Notes_List));
            Routing.RegisterRoute(nameof(NoteDetails), typeof(NoteDetails));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
        }
    }
}
