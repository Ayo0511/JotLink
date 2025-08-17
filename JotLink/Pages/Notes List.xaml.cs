using System.Collections.ObjectModel;
using System.Net.Http.Json;
 

namespace JotLink.Pages;

public partial class Notes_List : ContentPage
{
    public ObservableCollection<NoteFE> Notes;   
    private NoteFE? _selectedNote;   //might be becuase its nullable
    bool panalOpened = false;
    bool isAnimating = false;
 


    public Notes_List()                 
	{
        InitializeComponent();   //don                             
        Notes = new ObservableCollection<NoteFE>() ;           
        noteList.ItemsSource = Notes;
        bool isLight = Preferences.Get("Theme", "Light")=="Light";
        switchN.IsToggled=isLight;
        ApplyTheme(isLight);
       
	}

    public async void createNote(object sender, EventArgs e)
    {
        var tempNote = new NoteFE
        {      
         Title = !string.IsNullOrEmpty(Title_txt.Text) ? Title_txt.Text: "Unnamed Note",
            Content = "" // prevent null
        };

        var sharingService = new NoteSharingService();
        var sharedNote = await sharingService.ShareNoteAsync(tempNote);
        
        if (Connectivity.NetworkAccess == NetworkAccess.Internet)
        {           
            if (sharedNote == null)
            {
                await DisplayAlert("Error", "Failed to create note.", "OK");
                return;
            }
            Notes.Add(sharedNote);
            await addToDatabase(sharedNote);
            string link = $"https://jotlink.onrender.com/n/{sharedNote.PublicId}";
            await Clipboard.SetTextAsync(link);
            SimulateSelection(sharedNote);
        }
        else
        {
            await DisplayAlert("Error", "Could not share note online", "Ok");
        }
    }


    public async Task<NoteFE?> FetchNoteFromLink(string publicId)
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri("https://jotlink.onrender.com/") 
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
        sortByLastOpened();
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

           // noteList.SelectedItem=null;
            
            
            NoteDetails.loadedNote = selectedNote;
            Shell.Current.GoToAsync(nameof(NoteDetails));


        }            
        
    }


    public void sortByLastOpened()
    {
        var sorted = Notes.OrderByDescending(n => n.LastModified).ToList();

        Notes.Clear();
        foreach (var note in sorted)
        {
            Notes.Add(note);
        }
    }


    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        
        string keyword = e.NewTextValue?.ToLower() ?? "";

        if (string.IsNullOrWhiteSpace(keyword))
        {
            
            OnAppearing(); //reload notes
            return;
        }

        // Filter Notes collection in place:
        var filtered = Notes.Where(note =>
            (note.Title?.ToLower().Contains(keyword) ?? false) ||
            (note.Content?.ToLower().Contains(keyword) ?? false)
        ).ToList();

        Notes.Clear();

        foreach (var note in filtered)
        {
            Notes.Add(note);
        }
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
    





    //simulate event driven function for opening note
    private async void SimulateSelection(NoteFE noteToSelect)
    {
        if (noteToSelect == null) return;

        NoteDetails.loadedNote = noteToSelect;
        
        await Shell.Current.GoToAsync(nameof(NoteDetails));
    }

   
    private async void LoadNoteFromLink_Clicked(object sender, EventArgs e)
    {
        var fullLink = LinkEntry.Text?.Trim();
        if (string.IsNullOrEmpty(fullLink))
        {
            await DisplayAlert("Error", "Please enter a link.", "OK");
            return;
        }
        if (Connectivity.NetworkAccess is not NetworkAccess.Internet)
            await DisplayAlert("No Internet","Please check your internet connection and try again","Ok");

            // Extract the PublicId from the URL (assumes format https://yourdomain.com/n/{publicId})
           
            var parts = fullLink.Split('/');
            var publicId = parts.Last();

        var note = await FetchNoteFromLink(publicId);

        if (note == null)
        {
            await DisplayAlert("Not found", "Note not found or invalid link.", "OK");
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
        
        await addToDatabase(note);
        SimulateSelection(note);
    }





   async private void openMenu_Clicked(object sender, EventArgs e)
    {
        const int targetWidth = 700;
        const int step = 10;
        if (isAnimating)
            return;

        if (!OptionsPanel.IsVisible && panalOpened == false)   // Open Panel
        {
            OptionsPanel.IsVisible = true;
            noteListMenu.IsVisible = true;

            for (int i = 0; i <= targetWidth; i += step)
            {
                noteListMenu.WidthRequest = i;
                isAnimating = true;
                await Task.Delay(1);
            }
            panalOpened = true;
            isAnimating = false;
        }

        else if (panalOpened)                               // Close Panel
        {

            for (int i = targetWidth; i >= 0; i -= step)
            {
                noteListMenu.WidthRequest = i;
                isAnimating = true;
                await Task.Delay(1);
            }

            OptionsPanel.IsVisible = false;
            noteListMenu.IsVisible = false;
            panalOpened = false;
            isAnimating = false;
        }
    }



   


    private void Switch_Toggled(object sender, ToggledEventArgs e)
    {
        ApplyTheme(e.Value);//returns bool 
    }



    private void ApplyTheme(bool isLight)
    {
        if (Application.Current is not null)
        {          
            if (isLight==true)
            {
                Preferences.Set("Theme", "Light");
                Application.Current.UserAppTheme = AppTheme.Light;
                noteList.BackgroundColor = Colors.White;
                Title_txt.TextColor = Colors.White;
                grid.BackgroundColor = Colors.White;
                switchN.OnColor = Colors.MediumPurple;
                loadNoteBtn.BackgroundColor = Colors.MediumPurple;
                openMenu.BackgroundColor = Colors.MediumPurple;
                addNoteBtn.BackgroundColor = Colors.MediumPurple;
                delNoteBtn.Background = Colors.MediumPurple;
            }
            else
            {
                Preferences.Set("Theme", "Dark");
                Application.Current.UserAppTheme = AppTheme.Dark;
                noteList.BackgroundColor = Colors.Black;
                Title_txt.TextColor = Colors.Black;
                grid.BackgroundColor = Colors.Black;
            }
        }
    }

   
    #region
    private void openMenu_Released(object sender, EventArgs e)
    {
        openMenu.BackgroundColor = Colors.MediumPurple;
        
    }

    private void openMenu_Pressed(object sender, EventArgs e)
    {
        openMenu.BackgroundColor = Colors.Purple;
    }

    private void loadNoteBtn_Pressed(object sender, EventArgs e)
    {
        loadNoteBtn.BackgroundColor= Colors.Purple;
    }

    private void loadNoteBtn_Released(object sender, EventArgs e)
    {
        loadNoteBtn.BackgroundColor = Colors.MediumPurple;
    }

    private void addNoteBtn_Pressed(object sender, EventArgs e)
    {
       addNoteBtn.BackgroundColor = Colors.Purple;
    }

    private void addNoteBtn_Released(object sender, EventArgs e)
    {
        addNoteBtn.BackgroundColor = Colors.MediumPurple;
    }

    private void delNoteBtn_Pressed(object sender, EventArgs e)
    {
        delNoteBtn.Background = Colors.Purple;
    }

    private void delNoteBtn_Released(object sender, EventArgs e)
    {
        delNoteBtn.Background = Colors.MediumPurple;
    }


    #endregion 

     
}





public static class NoteLinkStore
{
    public static string? CurrentPublicId { get; set; }
}








//navigate to main page use
//Shell.Current.GoToAsync($"//nameof(Notes_List)") 
//Shell.Current.GoToAsync(nameof(NoteDetails));




