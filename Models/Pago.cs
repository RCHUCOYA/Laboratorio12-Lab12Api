namespace Lab12Api.Models;

public class Pago
{
    public int IdPago { get; set; }
    public int IdMatricula { get; set; }
    public decimal Monto { get; set; }
    public DateTime FechaPago { get; set; }
    public string MetodoPago { get; set; } = string.Empty;
    public string EstadoPago { get; set; } = string.Empty;

    public Matricula? Matricula { get; set; }
}
