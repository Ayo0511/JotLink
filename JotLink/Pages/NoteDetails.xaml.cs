using Microsoft.Maui;
using Microsoft.Maui.Controls;
using System;
using System.Diagnostics;
using System.IO;
 

namespace JotLink.Pages; 

public partial class NoteDetails : ContentPage               //, IQueryAttributable
{
    private NoteFE? _note;
    public static NoteFE? loadedNote;
    bool panalOpened= false;
    bool isAnimating = false;
    string? publicId;
    
    
   
   

    public NoteDetails()
    {      
        InitializeComponent();
       

        var process = Process.GetCurrentProcess();
       

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (NoteLinkStore.CurrentPublicId != null) 
        {
            publicId = NoteLinkStore.CurrentPublicId;
        }
       
        if (loadedNote != null)
        {
            _note = loadedNote;

            TitleEditor.Text = _note.Title;
            ContentEditor.Text = _note.Content;
        }
        setColor();
       ApplyCurrentTheme();
    }


    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("SelectedNote", out var noteObj) && noteObj is NoteFE note)
        {
            _note = note;

            // Populate the UI
            ContentEditor.Text = note.Content;
            TitleEditor.Text   = note.Title;
        }
    }

    public async void Return(object sender, EventArgs e)
    {

        if (_note == null) return;
        try
        {
            progressIndicator.IsVisible = true;
            _note.Title = TitleEditor.Text;
            _note.Content = ContentEditor.Text;
            _note.LastModified = DateTime.Now;

            // Save locally
            var connection = new SqLiteConnection().CreateConnection();

            var dto = new NotesDTO
            {
                Id = _note.Id,
                Title = _note.Title,
                Content = _note.Content,
                CreatedAt = _note.CreatedAt,
                LastModified = _note.LastModified,
                PublicId = _note.PublicId
            };
            if (dto.Content.Length < 5000)
            {
                await connection.UpdateAsync(dto);
               
                // Save remotely (update backend)
                var sharingService = new NoteSharingService();
                var updatedNote = await sharingService.UpdateNoteAsync(_note);
                if (updatedNote == null) { updatedNote = await sharingService.ShareNoteAsync(_note); }

                if (updatedNote == null)
                {
                    await DisplayAlert("Error", "Failed to update note on server.", "OK");
                }
                else
                {
                    _note = updatedNote; // update local note with server response if needed
                }

                await Shell.Current.GoToAsync("..");
            }
            else if (dto.Content.Length > 5000)
            {
                await DisplayAlert("Content Too Long",
                                   "Notes cannot exceed 5000 characters. Please shorten your note.","OK");

            }
        }
       
        finally { progressIndicator.IsVisible = false; }
    
        }
       
     



    public async void openMenu_Clicked(object sender, EventArgs e)
    {
        const int targetWidth = 700;
        const int step = 10;
        if (isAnimating)
            return;
        
        if (!OptionsPanel.IsVisible&&panalOpened==false)   // Open Panel
        {    
            OptionsPanel.IsVisible = true;
            optionsFrame.IsVisible = true;                  

            for (int i = 0; i <= targetWidth; i += step)
            {
                optionsFrame.WidthRequest = i;
                isAnimating = true;
                await Task.Delay(1);        
            }
            panalOpened = true;
            isAnimating= false;
        }

        else  if(panalOpened)                               // Close Panel
        {
            
            for (int i = targetWidth; i >= 0; i -= step)
            {
                optionsFrame.WidthRequest = i;
                isAnimating = true;
                await Task.Delay(1);
            }

            OptionsPanel.IsVisible = false;
            optionsFrame.IsVisible = false;
            panalOpened = false;
            isAnimating = false;
        }
    }



  
    private void ApplyCurrentTheme()
    {
        if (Application.Current is not null)
        {
            var current = Application.Current.UserAppTheme;
            if (current == AppTheme.Dark)
            {            
                Application.Current.UserAppTheme = AppTheme.Dark;
                rootLayout.BackgroundColor = Colors.Black;
                ContentEditor.TextColor = Colors.White;
                TitleEditor.TextColor = Colors.White;              
            }
            else if (current == AppTheme.Light) 
            {         
                Application.Current.UserAppTheme = AppTheme.Light;
                rootLayout.BackgroundColor = Colors.White;
                ContentEditor.TextColor = Colors.Black;
                TitleEditor.TextColor = Colors.Black;
            }
        }
    }


    private void ShareNote(object sender, EventArgs e)
    {
        string link = $"https://jotlink.onrender.com/n/{_note!.PublicId}"; //NULL forgiving operator !!!!
        Clipboard.SetTextAsync(link);
        DisplayAlert("Link Copied", "Link has been copied to your clipboard.", "OK");

    }

    private void downloadBtn_Clicked(object sender, EventArgs e)
    {       
        string fileName = $"{_note?.Title.Replace(" ", "_")}.txt";
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string filePath = Path.Combine(desktopPath, fileName);
        File.WriteAllText(filePath, _note?.Content);
        DisplayAlert("Downloaded", "Note Download To Desktop Completed", "Okay");       
    }

   


    public void setColor()
    {
        openMenu.BackgroundColor = Colors.MediumPurple;
        downloadBtn.BackgroundColor = Colors.MediumPurple;
        shareNoteBtn.Background = Colors.MediumPurple;
        returnBtn.BackgroundColor = Colors.MediumPurple;
    }

    #region
    private void shareNoteBtn_Pressed(object sender, EventArgs e)
    {
        shareNoteBtn.Background = Colors.Purple;
    }

    private void shareNoteBtn_Released(object sender, EventArgs e)
    {
        shareNoteBtn.Background = Colors.MediumPurple;
    }

    private void returnBtn_Pressed(object sender, EventArgs e)
    {
        returnBtn.BackgroundColor = Colors.Purple;
    }

    private void returnBtn_Released(object sender, EventArgs e)
    {
        returnBtn.BackgroundColor = Colors.MediumPurple;
    }
    private void downloadBtn_Released(object sender, EventArgs e)
    {
        downloadBtn.BackgroundColor = Colors.MediumPurple;
    }

    private void downloadBtn_Pressed(object sender, EventArgs e)
    {
        downloadBtn.BackgroundColor = Colors.Purple;
    }

    private void openMenu_Pressed(object sender, EventArgs e)
    {
        openMenu.BackgroundColor = Colors.Purple;
    }

    private void openMenu_Released(object sender, EventArgs e)
    {
        openMenu.BackgroundColor = Colors.MediumPurple;
    }

    #endregion  
}



