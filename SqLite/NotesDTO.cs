using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JotLink
{
   public  class NotesDTO//Data transfer object
    {
        [PrimaryKey]
        public Guid Id { get; set; } // Unique ID
        public string Title { get; set; } = string.Empty;//Name of the note/page
        public string Content { get; set; } = string.Empty; 
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }


         
    }


    
}
