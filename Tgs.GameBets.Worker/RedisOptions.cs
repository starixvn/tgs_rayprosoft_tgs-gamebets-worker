namespace Tgs.GameBets.Worker;

public record RedisOptions
{
	public string ConnectionString { get; set; } = "localhost";
}
