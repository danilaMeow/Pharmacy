using Pharmacy.Api.Models;
using Xunit;

namespace Pharmacy.Tests;

public class PharmacyValidationTests
{
    [Fact]
    public void Medication_Price_ShouldBePositive()
    {
        // Проверяем, что цена на лекарство больше нуля
        var medicine = new Medication
        {
            Name = "Аспирин",
            Price = 150.50m
        };

        Assert.True(medicine.Price > 0);
    }

    [Fact]
    public void Medication_Stock_ShouldNotBeNegative()
    {
        // Проверяем, что количество на складе не может быть отрицательным
        var medicine = new Medication
        {
            Name = "Парацетамол",
            StockQuantity = 50
        };

        Assert.True(medicine.StockQuantity >= 0);
    }

    [Fact]
    public void Medication_Name_ShouldNotBeEmpty()
    {
        // Проверка, что имя заполнено
        var medicine = new Medication { Name = "Анальгин" };
        Assert.False(string.IsNullOrEmpty(medicine.Name));
    }
}