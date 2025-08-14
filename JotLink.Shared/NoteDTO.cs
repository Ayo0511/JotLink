namespace JotLink.Shared
{
    public class NoteDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        public string PublicId { get; set; } = "";
      

    }
}
///