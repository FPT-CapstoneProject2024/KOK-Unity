using System;
using System.Collections.Generic;

namespace SU24SE069_PLATFORM_KAROKE_DataAccess.Models
{
    public partial class InstrumentSheet
    {
        public Guid InstrumentSheetId { get; set; }
        public string InstrumentSheetContent { get; set; } = null!;
        public int InstrumentType { get; set; }
        public Guid SongId { get; set; }

        public virtual Song Song { get; set; } = null!;
    }
}
