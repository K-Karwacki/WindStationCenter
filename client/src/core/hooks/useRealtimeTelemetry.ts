import { useEffect } from "react";
import { useAtom } from "jotai";
import { pushTelemetryAtom } from "@core/atoms/telemetryAtoms";
import { type StateleSSEClient } from "statele-sse";
import { type RealtimeClient } from "src/generated-ts-client";

export function useRealtimeTelemetry(sse: StateleSSEClient, realtimeClient: RealtimeClient) {
    const [, pushTelemetry] = useAtom(pushTelemetryAtom);

    useEffect(() => {
       sse.listen(
            async (id) => {
                console.log("Listening for telemetry with id:", id);
                return await realtimeClient.getTelemetryDataRealtime(id);
            },
            (data) => {
                data.forEach(telemetry => {
                    // console.log("Received telemetry:", telemetry);
                    pushTelemetry(telemetry);
                });
            }
        );
    }, []);
}