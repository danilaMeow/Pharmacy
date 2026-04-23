namespace Pharmacy.Api.Models;

public class Sale
{
    public int Id { get; set; }
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;
    public int Quantity { get; set; }
    public int MedicationId { get; set; }
    public Medication? Medication { get; set; }
}