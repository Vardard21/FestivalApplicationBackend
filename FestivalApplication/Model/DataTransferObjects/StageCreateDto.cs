using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FestivalApplication.Model.DataTransferObjects
{
    public class StageCreateDto
    {
        public int StageID { get; set; }
        public string StageName { get; set; }
        public bool StageActive { get; set; }
    }
}
