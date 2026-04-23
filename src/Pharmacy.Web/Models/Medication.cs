namespace Pharmacy.Web.Models;

public class Medication
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}

public class Sale
{
    public int Id { get; set; }
    public DateTime SaleDate { get; set; }
    public int Quantity { get; set; }
    public int MedicationId { get; set; }
    public Medication? Medication { get; set; }
}