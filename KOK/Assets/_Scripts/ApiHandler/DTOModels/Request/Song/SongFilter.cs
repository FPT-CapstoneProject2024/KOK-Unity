namespace KOK.ApiHandler.DTOModels
{
    public class SongFilter
    {
        public string SongName { get; set; } = string.Empty;
        public string SongCode { get; set; } = string.Empty;
        public SongStatus SongStatus { get; set; } = SongStatus.ENABLE;
        public string GenreName { get; set; }
        public string SingerName { get; set; }
        public string ArtistName { get; set; }
    }
}
