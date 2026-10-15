using CreditWorks.Domain.Exceptions;

namespace CreditWorks.Domain.Entities;

public sealed class Manufacturer
{
    public int Id { get; private set; }
    public string Name { get; private set; } = null!;

    private Manufacturer()
    {
        // Required by EF Core
    }

    public Manufacturer(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainValidationException(
                "Manufacturer name is required.");

        Name = name.Trim();
    }
}