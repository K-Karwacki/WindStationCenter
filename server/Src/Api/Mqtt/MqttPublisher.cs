using MQTTnet;

namespace Api.Mqtt;

public class MqttPublisher(IMqttClient client)
{

    public async Task PublishCommandAsync(string farmId, string turbineId, MqttPublishCommand command)
    {
        
    }
}

public record MqttPublishCommand;

public record StartTurbineCommand : MqttPublishCommand
{
    public readonly string Action = "start";
}

public record StopTurbineCommand : MqttPublishCommand
{
    public readonly string Action = "stop";
    public required string Reason { get; set; }
}

public record SetBladePitchCommand : MqttPublishCommand
{
    public readonly string Action = "setPitch";
    /// has to be between 0 and 30
    public required double Angle { get; set; }

    public SetBladePitchCommand(double angle)
    {
        if (angle < 0 || angle > 30)
            throw new ArgumentOutOfRangeException(nameof(angle), angle, "Angle has to be between 0 and 30");
        Angle = angle;
    }
}

public record SetReportingIntervalCommand : MqttPublishCommand
{
    public readonly string Action = "setInterval";
    ///  has to be between 1 and 60
    public required int Value { get; set; }
    public SetReportingIntervalCommand(int value)
    {
        if (value < 1 || value > 60)
            throw new ArgumentOutOfRangeException(nameof(value), value, "Value has to be between 1 and 60");
        Value = value;
    }
}