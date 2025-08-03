using System;
using System.IO;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace JotLink.Pages; 

public partial class NoteDetails : ContentPage, IQueryAttributable
{
    private Note _note;
    bool panalOpened= false;
    bool isAnimating = false;
    
   
   

    public NoteDetails()
    {
       
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("SelectedNote", out var noteObj) && noteObj is Note note)
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

        _note.Content = ContentEditor.Text;
        _note.Title = TitleEditor.Text;
        _note.LastModified = DateTime.Now;

        // Save to SQLite directly
        var connection = new SqLiteConnection().CreateConnection();

        var dto = new NotesDTO
        {
            Id = _note.Id,
            Title = _note.Title,
            Content = _note.Content,
            CreatedAt = _note.CreatedAt,
            LastModified = _note.LastModified
        };

        await connection.UpdateAsync(dto);

        await Shell.Current.GoToAsync("..");
    }



    private async void openMenu_Clicked(object sender, EventArgs e)
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



    private void changeTheme_Clicked(object sender, EventArgs e)
    {
        var current = Application.Current.UserAppTheme;      

        if (current == AppTheme.Dark)
        {
            changeTheme.Text = "Dark Mode";
            Application.Current.UserAppTheme = AppTheme.Light;
            rootLayout.BackgroundColor = Colors.White;
            ContentEditor.TextColor = Colors.Black;
            TitleEditor.TextColor = Colors.Black;

        }
        else
        {
            changeTheme.Text = "Light Mode";
            Application.Current.UserAppTheme = AppTheme.Dark;
            rootLayout.BackgroundColor = Colors.Black;
            ContentEditor.TextColor = Colors.White;
            TitleEditor.TextColor = Colors.White;
           
            //Title_txt.TextColor = Colors.Black;

        }

    }


}



