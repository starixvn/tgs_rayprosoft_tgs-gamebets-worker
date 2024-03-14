namespace Tgs.GameWagers.Settlement.Worker;

public record RedisOptions
{
	public string ConnectionString { get; set; } = "localhost";
}
