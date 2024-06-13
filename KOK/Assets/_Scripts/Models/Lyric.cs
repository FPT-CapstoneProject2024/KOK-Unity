using System;
using System.Collections.Generic;

namespace SU24SE069_PLATFORM_KAROKE_DataAccess.Models
{
    public partial class Lyric
    {
        public Guid LyricSheetId { get; set; }
        public string LyricSheetContent { get; set; } = null!;
        public Guid SongId { get; set; }

        public virtual Song Song { get; set; } = null!;
    }
}
