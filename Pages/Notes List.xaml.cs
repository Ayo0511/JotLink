using System.Collections.ObjectModel;

namespace JotLink.Pages;

public partial class Notes_List : ContentPage
{
    public ObservableCollection<Note> Notes;
    private Note _selectedNote;

  
    Note note = new Note("Hello!");
    Note note1 = new Note("meek");


    public Notes_List()
	{
        InitializeComponent();
       

        Notes = new ObservableCollection<Note>() ;   
        
        note.Content = "welcome stuff stuff stuff stuff stuf fstuff stuff stuff stuff";
        note1.Content = "meek mek me m ........";
        //Notes.Add(note);
        //Notes.Add(note1);

        noteList.ItemsSource = Notes;
	}


    public async void createNote(object sender, EventArgs e)
    {
        Note note = new Note{Title = "Unnamed Note"};
        Notes.Add(note);
        Title_txt.Text = null;


        var connection = new SqLiteConnection().CreateConnection();
        await connection.InsertAsync(new NotesDTO
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            CreatedAt = note.CreatedAt,
            LastModified = note.LastModified
        });

        SimulateSelection(note);
    }


  protected override async void OnAppearing()
 {
    base.OnAppearing();

    Notes.Clear();

    var connection = new SqLiteConnection().CreateConnection();
    await connection.CreateTableAsync<NotesDTO>();
    var dtos = await connection.Table<NotesDTO>().ToListAsync();

    foreach (var dto in dtos)
    {
        Notes.Add(new Note
        {
            Title = dto.Title,
            Content = dto.Content,
            CreatedAt = dto.CreatedAt,
            LastModified = dto.LastModified,
            Id = dto.Id
        });
    }
}
    public void updateNote() 
    {
        noteList.ItemsSource = null;
        noteList.ItemsSource = Notes;
    }


    public async void deleteNote(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Delete", "Are you sure you want to delete this note?", "Yes", "Cancel");

        if (confirm && _selectedNote != null)
        {
            // 1. Delete from SQLite
            var connection = new SqLiteConnection().CreateConnection();
            await connection.DeleteAsync<NotesDTO>(_selectedNote.Id);

            // 2. Remove from ObservableCollection (UI)
            Notes.Remove(_selectedNote);

            // 3. Clear reference
            _selectedNote = null;
            updateNote();
            OnAppearing();
            
        }
       
    }


    //delete and enter note both get triggered by click. fix it



    private void noteList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Note selectedNote)
        {
            
            _selectedNote = selectedNote;
            noteList.SelectedItem=null;

            Shell.Current.GoToAsync(nameof(NoteDetails), new Dictionary<string, object>
        {
            {"SelectedNote",selectedNote}
        }
           );
             

        }            
        
    }


    public void sortByLastOpened() 
    { 

    }


    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        

    }



    


    private void changeMode(object sender, EventArgs e)
    {
        var current = Application.Current.UserAppTheme;

      

        if (current == AppTheme.Dark)
        {
            themeButton.Text = "Dark Mode";            
            Application.Current.UserAppTheme = AppTheme.Light;
            noteList.BackgroundColor=Colors.White;
            
             
        }
        else
        {
            themeButton.Text = "Light Mode";
            Application.Current.UserAppTheme = AppTheme.Dark;
            noteList.BackgroundColor = Colors.Black;
            Title_txt.TextColor = Colors.Black;

        }
    }





    //simulate event driven function for opening note
    private void SimulateSelection(Note noteToSelect)
    {
        if (noteToSelect == null) return;

        var args = new SelectedItemChangedEventArgs(noteToSelect, 0);
        noteList_ItemSelected(this, args);
    }


    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox checkbox && checkbox.BindingContext is Note item) 
        {
            var note = Notes;
            int index = note.IndexOf(item);
            Notes.Move(index, 0);
            checkbox.IsVisible = false;           
        }
    }




    //navigate to main page use
    //Shell.Current.GoToAsync($"//nameof(Notes_List)") 
    //Shell.Current.GoToAsync(nameof(NoteDetails));
}



















