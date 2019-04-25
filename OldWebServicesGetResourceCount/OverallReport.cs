using System;
using System.Collections.Generic;

namespace ProjectorAutomation
{
   public class OverallReport
  {
        public string ReprortingUnit { get; set; }
        public DateTime PrintDateTime { get; set; }
        public string LocalInstance { get; set; }
        public Decimal Currency { get; set; }
        public string StartWeek { get; set; }
        public string EndWeek { get; set; }
        List<Person> People { get; set; }
    }
}
