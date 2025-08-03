using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Specialized;


namespace JotLink
{
    public class Note
    {
        public Guid Id { get;  set; } // Unique ID
        public string Title { get; set; } =string.Empty;//Name of the note/page
        public string Content { get; set; }=string.Empty;
        public string Snippet => Content?.Length > 200 ? Content.Substring(0, 200) + "..." : Content;
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        //formatted time
        public string CreatedAtFormatted => CreatedAt.ToString("hh:mm tt");
        public string LastModifiedFormatted => LastModified.ToString("hh:mm tt");


        public Note()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            LastModified = CreatedAt;
        }

        public Note(string title)
        {
            Title = title;
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            LastModified = CreatedAt;
        }

         

        


    }
}




//IMPLEMENT inotifyProperty changed. to reflect changes in notes as it happens