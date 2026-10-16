
using CreditWorks.Domain.Entities;
using CreditWorks.Domain.Exceptions;
using Xunit;

namespace CreditWorks.Domain.Tests.Entities;

public sealed class VehicleValidationTests
{
    [Theory]
    [InlineData(1886)]
    [InlineData(2000)]
    [InlineData(2020)]
    public void Constructor_ValidYear_CreatesVehicle(int year)
    {
        var vehicle = new Vehicle(
            "John Smith", 1, year, 1500.50m);

        Assert.Equal(year, vehicle.YearOfManufacture);
    }

    [Fact]
    public void Constructor_CurrentYear_IsValid()
    {
        var year = DateTime.UtcNow.Year;

        var vehicle = new Vehicle(
            "John Smith", 1, year, 1500m);

        Assert.Equal(year, vehicle.YearOfManufacture);
    }

    [Theory]
    [InlineData(-2020)]
    [InlineData(0)]
    [InlineData(1800)]
    public void Constructor_InvalidYear_Throws(int year)
    {
        Assert.Throws<DomainValidationException>(() =>
            new Vehicle("John Smith", 1, year, 1500m));
    }

    [Fact]
    public void Constructor_FutureYear_Throws()
    {
        var year = DateTime.UtcNow.Year + 1;

        Assert.Throws<DomainValidationException>(() =>
            new Vehicle("John Smith", 1, year, 1500m));
    }

    [Theory]
    [InlineData("John Smith")]
    [InlineData("Mary")]
    public void Constructor_ValidOwnerName_Succeeds(string name)
    {
        var vehicle = new Vehicle(
            name, 1, 2020, 1500m);

        Assert.Equal(name, vehicle.OwnerName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Constructor_BlankOwnerName_Throws(string name)
    {
        Assert.Throws<DomainValidationException>(() =>
            new Vehicle(name, 1, 2020, 1500m));
    }

    [Fact]
    public void Constructor_OwnerNameTooLong_Throws()
    {
        var name = new string('A', 151);

        Assert.Throws<DomainValidationException>(() =>
            new Vehicle(name, 1, 2020, 1500m));
    }

    [Fact]
    public void Constructor_OwnerName_IsTrimmed()
    {
        var vehicle = new Vehicle(
            "  John Smith  ", 1, 2020, 1500m);

        Assert.Equal("John Smith", vehicle.OwnerName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_InvalidManufacturer_Throws(int id)
    {
        Assert.Throws<DomainValidationException>(() =>
            new Vehicle("John Smith", id, 2020, 1500m));
    }

    [Theory]
    [InlineData("1500")]
    [InlineData("1500.5")]
    [InlineData("1500.55")]
    public void Constructor_ValidWeight_Succeeds(string value)
    {
        var weight = decimal.Parse(
            value,
            System.Globalization.CultureInfo.InvariantCulture);

        var vehicle = new Vehicle(
            "John Smith", 1, 2020, weight);

        Assert.Equal(weight, vehicle.Weight);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-100")]
    [InlineData("1500.555")]
    [InlineData("10000000000000000")]
    public void Constructor_InvalidWeight_Throws(string value)
    {
        var weight = decimal.Parse(
            value,
            System.Globalization.CultureInfo.InvariantCulture);

        Assert.Throws<DomainValidationException>(() =>
            new Vehicle("John Smith", 1, 2020, weight));
    }
}
