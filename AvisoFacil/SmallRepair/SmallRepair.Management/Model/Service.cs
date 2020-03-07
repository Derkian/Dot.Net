using SmallRepair.Management.Enum;


namespace SmallRepair.Management.Model
{
    public class Service
    {
        public int IdService { get; set; }

        public int IdPart { get; set; }

        public Part Part { get; set; }

        public EnmServiceType ServiceType { get; set; }

        public double Time { get; set; }

        public double MaterialValue { get; set; }
    }
}
