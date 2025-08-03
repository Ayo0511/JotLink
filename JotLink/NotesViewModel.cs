using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JotLink;
using SQLite;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

public partial class NotesViewModel : ObservableObject
{
    private readonly SqLiteConnection _connection;

    public ObservableCollection<NoteFE> Notes { get; } = new();

    public NotesViewModel(SqLiteConnection connection)
    {
        _connection = connection;       
        LoadNotesCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadNotes()
    {
        ISQLiteAsyncConnection database = _connection.CreateConnection();
        var noteDto = await database.Table<NotesDTO>().ToListAsync();

        Notes.Clear();
        foreach (var dto in noteDto)
        {
            Notes.Add(new NoteFE("Unnamed NoteFE")); // Replace with mapping logic if needed
        }
    }

    [RelayCommand]
    public async Task AddNoteAsync(NoteFE note)
    {
        var db = _connection.CreateConnection();
        await db.InsertAsync(new NotesDTO
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            CreatedAt = note.CreatedAt,
            LastModified = note.LastModified
        });
        Notes.Add(note);
    }




}