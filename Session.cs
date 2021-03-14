using System;
using System.Collections.Generic;
using System.Text;

namespace RouleteIRS
{
    public class Session
    {
        public decimal UnitAmount { get; set; }
        public decimal BetPerDozen { get; set; }
        public int SpinNumber { get; set; }
        public decimal Bankroll { get; set; }
        public List<int> Dozens { get; set; }
        public string LastResult { get; set; }
        public int LastUnits { get; set; }
        public int NumberOfConsecutiveLosses { get; set; }
        public int CancellationWay { get; set; }
        public List<int> BettingUnits { get; set; }

        public Session()
        {
            Dozens = new List<int>();
            BettingUnits = new List<int>();
        }

        public int GetDozenNumber(int number)
        {
            if (number >= 1 && number <= 12)
                return 1;
            else if (number >= 13 && number <= 24)
                return 2;
            else if (number >= 25 && number <= 36)
                return 3;

            return 0;
        }
    }
}
