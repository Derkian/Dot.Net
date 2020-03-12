using SmallRepair.Management.Enum;


namespace SmallRepair.Management.Model
{
    public class Service
    {
        /// <summary>
        /// Id do serviço
        /// </summary>
        public int IdService { get; set; }

        /// <summary>
        /// Id Peca 
        /// </summary>
        public int IdPart { get; set; }

        /// <summary>
        /// Peca
        /// </summary>
        public Part Part { get; set; }

        /// <summary>
        /// Tipo do serviço
        /// </summary>
        public EnmServiceType ServiceType { get; set; }        

        /// <summary>
        /// Tempo do serviço
        /// </summary>
        public double Time { get; set; }

        /// <summary>
        /// Valor por hora
        /// </summary>
        public double ValuePerHour { get; set; }

        /// <summary>
        /// Time * BaremoTime.Hour
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Valor do material
        /// </summary>
        public double MaterialValue { get; set; }

        /// <summary>
        /// MaterialValue + Value
        /// </summary>
        public double Total { get; set; }
    }
}
