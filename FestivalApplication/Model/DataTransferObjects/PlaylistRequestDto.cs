using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FestivalApplication.Model.DataTransferObjects
{
    public class PlaylistRequestDto
    {
        //public int PlaylistID { get; set; }
        //public int TrackNumber { get; set; }
        //public DateTime StartOfCurrentSong { get; set; }
        //public Array PreviousSong { get; set; }
        //public Array NextSong { get; set; }

        public int Id { get; set; }
        public string TrackName { get; set; }
        public string TrackSource { get; set; }
        public int Length { get; set; }

    }
}