using SQLite;

namespace JotLink
{
    public partial class App : Application
    {
        private readonly SqLiteConnection _connection;
        public App(SqLiteConnection connection)
        {
            InitializeComponent();
            _connection = connection;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        protected override async void OnStart()
        {
            ISQLiteAsyncConnection database = _connection.CreateConnection();
            
            await database.CreateTableAsync<NotesDTO>();
            
            base.OnStart();
        }
    }
}