namespace TransporteAPI.Models
{
    public class Vehiculo
    {
        public int Id { get; set; }
        public string? Marca { get; set; } = string.Empty;
        public string? Modelo { get; set; } = string.Empty;
        public int? Anio { get; set; }
    }
}
