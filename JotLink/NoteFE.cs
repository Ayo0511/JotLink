using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Specialized;
 



namespace JotLink
{

    
    [Serializable]
    public class NoteFE
    {
        public Guid Id { get;  set; } // Unique ID
        public string Title { get; set; } =string.Empty;//Name of the note/page
        public string Content { get; set; }=string.Empty;
        public string? Snippet => Content?.Length > 200 ? Content.Substring(0, 200) + "..." : Content;
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        public string PublicId { get; set; }

     
        


        //formatted time
        public string CreatedAtFormatted => CreatedAt.ToString("hh:mm tt");
        public string LastModifiedFormatted => LastModified.ToString("hh:mm tt");

       
        public NoteFE()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            LastModified = CreatedAt;
            PublicId = GenerateShortId();
        }

        public NoteFE(string title)
        {
            Title = title;
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            LastModified = CreatedAt;
            PublicId = GenerateShortId();
        }

        private static string GenerateShortId(int length = 8)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


    }
}




//IMPLEMENT inotifyProperty changed. to reflect changes in notes as it happens