using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOK.ApiHandler.DTOModels
{
    [Serializable]
    public class CreateVoiceAudioRequest
    {
        public string VoiceUrl { get; set; } = null!;
        public double DurationSecond { get; set; }
        public float StartTime { get; set; }
        public float EndTime { get; set; }
        public Guid MemberId { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
