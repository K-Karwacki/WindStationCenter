import { type RealtimeClient } from "../../generated-ts-client";
import { type StateleSSEClient } from "statele-sse";
import { useRealtimeTelemetry } from "@core/hooks/useRealtimeTelemetry";
import { useRealtimeAlerts } from "@core/hooks/useRealtimeAlerts";

interface RealtimeComponentProps {
    sse: StateleSSEClient;
    realtimeClient: RealtimeClient;
}

export default function RealtimeComponent(props: RealtimeComponentProps) {
    useRealtimeTelemetry(props.sse, props.realtimeClient);
    useRealtimeAlerts(props.sse, props.realtimeClient);
    return <></>;
}