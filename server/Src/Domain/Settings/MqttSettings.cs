namespace Domain.Settings;

public class MqttSettings
{
    public required string Broker { get; init; }
    public int Port { get; init; }

}