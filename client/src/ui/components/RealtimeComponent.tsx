import { pushTelemetryAtom } from "@core/atoms/telemetryAtoms";
import { useAtom } from "jotai";
import { useEffect } from "react";
import { RealtimeClient } from "../../generated-ts-client";
import { StateleSSEClient } from "statele-sse";

const API_ENDPOINT = "api/realtime";

const sse = new StateleSSEClient(`${API_ENDPOINT}/sse`);
const realtimeClient = new RealtimeClient();


export default function RealtimeComponent() {
    const [, pushTelemetry] = useAtom(pushTelemetryAtom);

    useEffect(() => {
    sse.listen(async (id) => {
      const result = await realtimeClient.getTelemetry(id);
      return result;
    }, (data) => {
        data.forEach(telemetry => {
            pushTelemetry(telemetry);
            console.log("Received telemetry:", telemetry); // Debug log for received telemetry
        });
    });
    }, []);

    return (
        <div>
            <h2>Realtime Component</h2>
            <p>This component will handle real-time updates using the generated RealtimeClient.</p>
        </div>
    );
}