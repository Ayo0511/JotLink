
namespace JotLink
{
    public partial class MainPage : ContentPage
    {
        int count = 100;
        Button myButton;
        NoteFE note= new NoteFE("stuff");
        
        public MainPage()
        {
            InitializeComponent();
            myButton=makeButton("",Colors.White,Colors.Violet,10);
            MainLayout.Children.Add(myButton);
            note.Content = "ccvcvcvc";
            
        }

        private void OnCounterClicked(object? sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);


            // DisplayAlert("Clicked!", "You clicked the C# button.", "Close");
            
        }

        public Button makeButton( string buttonTxt, Color color, Color backColor, int radius) 
        {
           Button button = new Button
            {
                Text = buttonTxt,
                BackgroundColor = backColor,
                TextColor = color,
                CornerRadius = radius, // Makes the button rounded
                Padding = new Thickness(10)
            };
            return button;
        }
    }
}
