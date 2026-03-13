import { useEffect } from "react";
import { useSetAtom } from "jotai";
import { clearTelemetryAtom, setTelemetryBulkAtom } from "@core/atoms/telemetryAtoms";
import { type StateleSSEClient } from "statele-sse";
import { type RealtimeClient } from "src/generated-ts-client";

export function useRealtimeTelemetry(sse: StateleSSEClient, realtimeClient: RealtimeClient) {
    const clearAll = useSetAtom(clearTelemetryAtom);
    const setBulk = useSetAtom(setTelemetryBulkAtom);

    useEffect(() => {
       sse.listen(
            async (id) => {
                console.log("Listening for telemetry with id:", id);
                return (await realtimeClient.getTelemetryDataRealtime(id));
            },
            (data) => {
                // Clear existing telemetry and set incoming data in bulk to avoid
                // iterating and triggering many updates which cause lag.
                clearAll();
                setBulk(data ?? []);
            }
        );
    }, []);
}