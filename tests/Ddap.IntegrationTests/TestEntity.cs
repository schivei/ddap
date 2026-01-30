namespace Ddap.IntegrationTests;

public class TestEntity
{
    public required string Id { get; set; }
    public string? Name { get; set; }
    public int Value { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}
