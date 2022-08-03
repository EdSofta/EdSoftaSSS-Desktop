using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdSofta.Models
{
    public class PracticeMode
    {
        public bool withTimer { get; set; }
        public double timeValue { get; set; }
        public double timeUsed { get; set; }

        public PracticeMode(bool withTimer, double timeValue)
        {
            this.withTimer = withTimer;
            this.timeValue = timeValue;
        }

        public PracticeMode()
        {

        }

    }
}
