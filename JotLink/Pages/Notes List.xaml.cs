using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace JotLink.Pages;

public partial class Notes_List : ContentPage
{
    public ObservableCollection<NoteFE> Notes;
    private NoteFE _selectedNote;

  
    NoteFE note = new NoteFE("Hello!");
    NoteFE note1 = new NoteFE("meek");


    public Notes_List()                  //create backend in same  project 
	{
        InitializeComponent();   //don
       

        Notes = new ObservableCollection<NoteFE>() ;   
        
        note.Content = "welcome stuff stuff stuff stuff stuf fstuff stuff stuff stuff";
        note1.Content = "meek mek me m ........";
        //Notes.Add(note);
        //Notes.Add(note1);

        noteList.ItemsSource = Notes;
	}


    public async void createNote(object sender, EventArgs e)
    {
        var tempNote = new NoteFE
        {
            Title = "Unnamed NoteFE",
            Content = "" // prevent null
        };

        var sharingService = new NoteSharingService();
        var sharedNote = await sharingService.ShareNoteAsync(tempNote);

        if (sharedNote == null)
        {
            await DisplayAlert("Error", "Failed to create note.", "OK");
            return;
        }

        Notes.Add(sharedNote);

        await addToDatabase(sharedNote);

        string link = $"https://jotlink.onrender.com/n/{sharedNote.PublicId}";
        await Clipboard.SetTextAsync(link);
        await DisplayAlert("Link Copied", "The note link has been copied to your clipboard.", "OK");

        SimulateSelection(sharedNote);
    }


    public async Task<NoteFE?> FetchNoteFromLink(string publicId)
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri("https://jotlink.onrender.com/")// must match backend
        };

        var response = await client.GetAsync($"n/{publicId}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<NoteFE>();
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
        Notes.Add(new NoteFE
        {
            Title = dto.Title,
            Content = dto.Content,
            CreatedAt = dto.CreatedAt,
            LastModified = dto.LastModified,
            Id = dto.Id,
            PublicId= dto.PublicId
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
        if (e.SelectedItem is NoteFE selectedNote)
        {
            NoteLinkStore.CurrentPublicId = selectedNote.PublicId;
            _selectedNote = selectedNote;
            noteList.SelectedItem=null;

            NoteDetails.loadedNote = selectedNote;
            Shell.Current.GoToAsync(nameof(NoteDetails));


        }            
        
    }
    

    public void sortByLastOpened() 
    { 

    }


    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        

    }


    public async Task addToDatabase(NoteFE note)
    {
        var connection = new SqLiteConnection().CreateConnection();

        var dto = new NotesDTO
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            CreatedAt = note.CreatedAt,
            LastModified = note.LastModified,
            PublicId= note.PublicId
        };

        await connection.InsertOrReplaceAsync(dto);
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
    private async void SimulateSelection(NoteFE noteToSelect)
    {
        if (noteToSelect == null) return;

        NoteDetails.loadedNote = noteToSelect;
        await Shell.Current.GoToAsync(nameof(NoteDetails));
    }

    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is CheckBox checkbox && checkbox.BindingContext is NoteFE item) 
        {
            var note = Notes;
            int index = note.IndexOf(item);
            Notes.Move(index, 0);
            checkbox.IsVisible = false;           
        }
    }

    private async void LoadNoteFromLink_Clicked(object sender, EventArgs e)
    {
        var fullLink = LinkEntry.Text?.Trim();
        if (string.IsNullOrEmpty(fullLink))
        {
            await DisplayAlert("Error", "Please enter a link.", "OK");
            return;
        }

        // Extract the PublicId from the URL (assumes format https://yourdomain.com/n/{publicId})
        var parts = fullLink.Split('/');
        var publicId = parts.Last();

        var note = await FetchNoteFromLink(publicId);

        if (note == null)
        {
            await DisplayAlert("Not found", "NoteFE not found or invalid link.", "OK");
            return;
        }

        // Add or update note in your ObservableCollection
        var existingNote = Notes.FirstOrDefault(n => n.PublicId == note.PublicId);
        if (existingNote != null)
        {
            // Update existing note
            var index = Notes.IndexOf(existingNote);
            Notes[index] = note;
        }
        else
        {
            // Add new note
            Notes.Add(note);
        }

        // Optionally save to local DB here as well

       await addToDatabase(note);
        SimulateSelection(note);
    }


   

    //navigate to main page use
    //Shell.Current.GoToAsync($"//nameof(Notes_List)") 
    //Shell.Current.GoToAsync(nameof(NoteDetails));
}





public static class NoteLinkStore
{
    public static string? CurrentPublicId { get; set; }
}













