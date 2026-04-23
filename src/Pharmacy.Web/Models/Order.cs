namespace Pharmacy.Web.Models;

public enum OrderStatus { Pending, Approved, Rejected }

public class Order
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int MedicationId { get; set; }
    public Medication? Medication { get; set; }
    public int Quantity { get; set; }
    public OrderStatus Status { get; set; }
    public string? AdminComment { get; set; }
}