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
    public required double Angle { get; set; }

    public SetBladePitchCommand(double angle)
    {
    }
}

public record SetReportingIntervalCommand : MqttPublishCommand
{
    public readonly string Action = "setInterval";
    public required int Value { get; set; }
}