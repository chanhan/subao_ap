using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedules.SourceModel.Baseball
{
    public class PlayOff
    {
        public PlayOff(DateTime gameTime, string away, string home)
        {
            this.GameTime = gameTime;
            this.Away = away;
            this.Home = home;
        }

        public DateTime GameTime { private set; get; }

        public string OriginalGameDate { set; get; }
        public string OriginalGameTime { set; get; }

        public string Away { private set; get; }
        public string Home { private set; get; }
    }
}
