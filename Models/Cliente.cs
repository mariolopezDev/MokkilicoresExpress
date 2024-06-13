namespace MokkilicoresExpress.Models
{
    public class Cliente
    {
        public string Identificacion { get; set; }
        public string NombreCompleto { get; set; }
        public string Provincia { get; set; }
        public string Canton { get; set; }
        public string Distrito { get; set; }
        public decimal DineroCompradoTotal { get; set; }
        public decimal DineroCompradoUltimoAnio { get; set; }
        public decimal DineroCompradoUltimosSeisMeses { get; set; }
    }
}
