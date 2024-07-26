using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace KOK
{
    public class DemoSongManager : NetworkBehaviour
    {
        private NetworkRunner networkRunner;

        private void Awake()
        {
        }

        private void OnEnable()
        {
            networkRunner = FindAnyObjectByType<NetworkRunner>();
        }

        private void Start()
        {
            networkRunner = FindAnyObjectByType<NetworkRunner>();
        }

        

        public static DemoSong GetSongBySongCode(string songCode)
        {
            return songs.FirstOrDefault(x => x.songCode.Equals(songCode.ToString()));
        }

        public static List<DemoSong> songs = new List<DemoSong>()
        {
            new("S001","Buồn Thì Cứ Khóc Đi", "https://firebasestorage.googleapis.com/v0/b/kok-unity.appspot.com/o/KaraokeVideos%2FBu%E1%BB%93n%20Th%C3%AC%20C%E1%BB%A9%20Kh%C3%B3c%20%C4%90i.mp4?alt=media&token=f46adca8-0cd5-4860-896c-dd8efb3bf05a", "Lynk Lee"),
            new("S002","Cắt Đôi Nỗi Sầu", "https://firebasestorage.googleapis.com/v0/b/kok-unity.appspot.com/o/KaraokeVideos%2FC%E1%BA%AFt%20%C4%91%C3%B4i%20n%E1%BB%97i%20s%E1%BA%A7u.mp4?alt=media&token=783ab525-e7c2-4c17-86c0-e3874daa95e3", "Tăng Duy Tân"),
            new("S003","Luôn Yêu Đời", "https://firebasestorage.googleapis.com/v0/b/kok-unity.appspot.com/o/KaraokeVideos%2FLu%C3%B4n%20Y%C3%AAu%20%C4%90%E1%BB%9Di.mp4?alt=media&token=36b7aba5-4a81-427d-972f-8c34c66e22dc", "Đen, Cheng, Low G"),
            new("S004","Một Đêm Say", "https://firebasestorage.googleapis.com/v0/b/kok-unity.appspot.com/o/KaraokeVideos%2FM%E1%BB%99t%20%C4%90%C3%AAm%20Say.mp4?alt=media&token=ff66ba2c-ef79-4900-9b0d-c672c3ce7c6e", "Thịnh Suyz"),
            new("S005","Nàng Thơ", "https://firebasestorage.googleapis.com/v0/b/kok-unity.appspot.com/o/KaraokeVideos%2FN%C3%A0ng%20Th%C6%A1.mp4?alt=media&token=7297d708-5620-468a-9ab5-3168b591b79a", "Hoàng Dũng"),
            new("S006","Nơi Tình Yêu Bắt Đầu", "https://firebasestorage.googleapis.com/v0/b/kok-unity.appspot.com/o/KaraokeVideos%2FN%C6%A1i%20T%C3%ACnh%20Y%C3%AAu%20B%E1%BA%AFt%20%C4%90%E1%BA%A7u.mp4?alt=media&token=85471455-5e3e-408d-8c47-9de392246f33", "Tiến Minh"),
            new("S007","Thu Cuối", "https://firebasestorage.googleapis.com/v0/b/kok-unity.appspot.com/o/KaraokeVideos%2FThu%20Cu%E1%BB%91i.mp4?alt=media&token=f0e40b5a-5273-4ab9-b163-f823ed8df002", "Yanbi"),
            new("S008","Tháng Tư Là Lời Nói Dối Của Em", "https://firebasestorage.googleapis.com/v0/b/kok-unity.appspot.com/o/KaraokeVideos%2FTh%C3%A1ng%20T%C6%B0%20L%C3%A0%20L%E1%BB%9Di%20N%C3%B3i%20D%E1%BB%91i%20C%E1%BB%A7a%20Em.mp4?alt=media&token=54486377-6b10-407f-a836-dc25ea0da24d", "Phạm Toàn Thắng"),
            new("S009","Thắc Mắc", "https://firebasestorage.googleapis.com/v0/b/kok-unity.appspot.com/o/KaraokeVideos%2FTh%E1%BA%AFc%20m%E1%BA%AFc.mp4?alt=media&token=0c561ced-fd0b-4a56-b607-0f81ee290f4a", "Thịnh Suy"),
            new("S010","Trốn Tìm", "https://firebasestorage.googleapis.com/v0/b/kok-unity.appspot.com/o/KaraokeVideos%2FTr%E1%BB%91n%20T%C3%ACm.mp4?alt=media&token=5f75be8b-6dde-451f-b1f1-33535c8601f1", "Đen"),
            new("S011","Tình Đắng Như Ly Cà Phê", "https://firebasestorage.googleapis.com/v0/b/kok-unity.appspot.com/o/KaraokeVideos%2FT%C3%ACnh%20%C4%90%E1%BA%AFng%20T%C3%ACnh%20%C4%90%E1%BA%AFng%20Nh%C6%B0%20Ly%20C%C3%A0%20Ph%C3%AA.mp4?alt=media&token=fc5bc068-6237-4acc-96ec-321640348db6", "Nân, Ngơ"),
            new("S012","Túy Âm", "https://firebasestorage.googleapis.com/v0/b/kok-unity.appspot.com/o/KaraokeVideos%2FT%C3%BAy%20%C3%82m.mp4?alt=media&token=77cd5236-39e2-4120-95dd-ec64ed64cc3e", "Xesi"),
            new("S013","Waiting For You", "https://firebasestorage.googleapis.com/v0/b/kok-unity.appspot.com/o/KaraokeVideos%2FWaiting%20For%20You.mp4?alt=media&token=f7324ade-7d0e-4752-a9b3-de8bc0706d48", "Mono"),
            new("S014","Vết Mưa", "https://firebasestorage.googleapis.com/v0/b/kok-unity.appspot.com/o/KaraokeVideos%2FV%E1%BA%BFt%20M%C6%B0a.mp4?alt=media&token=3ecf024a-c70e-40f8-bdf2-366d467732c1", "Vũ Cát Tường"),
        
        };
    }

    public class DemoSong
    {
        public string songCode;
        public string songName;
        public string songURL;
        public string songArtist;

        public DemoSong(string songCode,string songName, string songURL, string songArtist)
        {
            this.songCode = songCode;
            this.songName = songName;
            this.songURL = songURL;
            this.songArtist = songArtist;
        }

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }
}
