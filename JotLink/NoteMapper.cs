using JotLink.Shared;

namespace JotLink
{
    public static class NoteMapper
    {
       static  int test = 1000;
        static int temmmst = 1000;


        public static NoteDTO ToDTO(NoteFE noteFE)
        {
            if (noteFE == null) return null!;
            return new NoteDTO
            {
                Id = noteFE.Id,
                Title = noteFE.Title,
                Content = noteFE.Content,
                CreatedAt = noteFE.CreatedAt,
                LastModified = noteFE.LastModified,
                PublicId = noteFE.PublicId
            };
        }

        public static NoteFE ToFE(NoteDTO dto)
        {
            if (dto == null) return null!;
            return new NoteFE
            {
                Id = dto.Id,
                Title = dto.Title,
                Content = dto.Content,
                CreatedAt = dto.CreatedAt,
                LastModified = dto.LastModified,
                PublicId = dto.PublicId
            };
        }

        public static NotesDTO ToLocalDTO(NoteFE noteFE)
        {
            if (noteFE == null) return null!;
            return new NotesDTO
            {
                Id = noteFE.Id,
                Title = noteFE.Title,
                Content = noteFE.Content,
                CreatedAt = noteFE.CreatedAt,
                LastModified = noteFE.LastModified,
                PublicId = noteFE.PublicId
            };
        }

        public static NoteFE FromLocalDTO(NotesDTO dto)
        {
            if (dto == null) return null!;
            return new NoteFE
            {
                Id = dto.Id,
                Title = dto.Title,
                Content = dto.Content,
                CreatedAt = dto.CreatedAt,
                LastModified = dto.LastModified,
                PublicId = dto.PublicId
            };
        }

        
    }
}