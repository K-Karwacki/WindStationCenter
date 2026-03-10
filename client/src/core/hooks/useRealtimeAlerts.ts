import { useEffect } from "react";
import { useAtom } from "jotai";
import { type StateleSSEClient } from "statele-sse";
import { type RealtimeClient } from "src/generated-ts-client";
import { pushTelemetryAlertAtom } from "@core/atoms/telemetryAletrsAtoms";

export function useRealtimeAlerts(sse: StateleSSEClient, realtimeClient: RealtimeClient) {
    const [, pushTelemetryAlerts] = useAtom(pushTelemetryAlertAtom);

    useEffect(() => {
       sse.listen(
            async (id) => {
                console.log("Listening for alerts with id:", id);
                return await realtimeClient.getTelemetryAlertsRealtime(id);
            },
            (data) => {
                data.forEach(alert => {
                    pushTelemetryAlerts(alert);
                });
            }
        );
    }, []);
}