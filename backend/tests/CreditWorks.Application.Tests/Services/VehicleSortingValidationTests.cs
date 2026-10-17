
using CreditWorks.Application.Interfaces;
using CreditWorks.Application.Services;
using CreditWorks.Domain.Exceptions;
using Moq;
using Xunit;

namespace CreditWorks.Application.Tests.Services;

public sealed class VehicleSortingValidationTests
{
    private readonly Mock<IVehicleRepository> _vehicleRepository;
    private readonly Mock<IManufacturerRepository> _manufacturerRepository;
    private readonly Mock<ICategoryRepository> _categoryRepository;

    private readonly VehicleService _service;

    public VehicleSortingValidationTests()
    {
        _vehicleRepository = new Mock<IVehicleRepository>();
        _manufacturerRepository = new Mock<IManufacturerRepository>();
        _categoryRepository = new Mock<ICategoryRepository>();

        _service = new VehicleService(
            _vehicleRepository.Object,
            _manufacturerRepository.Object,
            _categoryRepository.Object);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("price")]
    [InlineData("category")]
    [InlineData("")]
    public async Task GetAll_InvalidSortBy_Throws(string sortBy)
    {
        // Act
        var action = () => _service.GetAllAsync(
            sortBy, "asc");

        // Assert
        await Assert.ThrowsAsync<DomainValidationException>(
            action);

        // Invalid requests must not reach the database.
        _vehicleRepository.Verify(
            r => r.GetAllAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("ascending")]
    [InlineData("descending")]
    [InlineData("")]
    public async Task GetAll_InvalidDirection_Throws(
        string direction)
    {
        // Act
        var action = () => _service.GetAllAsync(
            "weight", direction);

        // Assert
        await Assert.ThrowsAsync<DomainValidationException>(
            action);

        _vehicleRepository.Verify(
            r => r.GetAllAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
