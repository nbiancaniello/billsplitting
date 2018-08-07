using System;
using System.Collections;
using System.Collections.Generic;

namespace BillSplittingPoco
{
    public class BillPoco
    {
        public List<int> members { get; set; }
        public List<int> bills { get; set; }
        public List<decimal> amount { get; set; }
    }
}
