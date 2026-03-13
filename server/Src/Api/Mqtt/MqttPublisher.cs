using System.Text.Json;
using Mqtt.Controllers;
using MQTTnet;
using MQTTnet.Protocol;

namespace Api.Mqtt;

public class MqttPublisher(IMqttClientService client)
{

    public async Task PublishCommandAsync(string turbineId, MqttPublishCommand command)
    {
        var topic = $"farm/my-awesome-farm/windmill/{turbineId}/command";
        var payload = JsonSerializer.Serialize((object)command, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = true
        });

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();
        
        await client.PublishAsync(topic, payload);
    }
}

public record MqttPublishCommand;

public record StartTurbineCommand : MqttPublishCommand
{
    public string Action { get; init; } = "start";
}

public record StopTurbineCommand : MqttPublishCommand
{
    public string Action { get; init; } = "stop";
    public required string Reason { get; set; } = "No reason provided";
}

public record SetBladePitchCommand : MqttPublishCommand
{
    public string Action { get; init; } = "setPitch";
    /// has to be between 0 and 30
    public required double Angle { get; set; }
}

public record SetReportingIntervalCommand : MqttPublishCommand
{
    public string Action { get; init; } = "setInterval";
    ///  has to be between 1 and 60
    public required int Value { get; set; }
}