namespace KarambaConnect.S2K
{
    public class CroSecFamilyName
    {
        public string FB { get; set; }
        public string Box { get; set; }
        public string H { get; set; }
        public string T { get; set; }
        public string L { get; set; }
        public string Pipe { get; set; }
        public string Circle { get; set; }
        public string Other { get; set; }

        public static CroSecFamilyName Default()
        {
            return new CroSecFamilyName
            {
                Box = "FH-Box",
                Circle = "FH-Circle",
                FB = "FH-FB",
                H = "FH-H",
                L = "FH-L",
                Pipe = "FH-Pipe",
                T = "FH-T",
                Other = "FH-Other"
            };
        }

        public override string ToString()
        {
            return $"Box:{Box}, H:{H}, Circle:{Circle}, Pipe:{Pipe}, FB:{FB}, L:{L}, T:{T}, Other:{Other}";
        }
    }
}