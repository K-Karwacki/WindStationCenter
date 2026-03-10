import { atom } from 'jotai';
import { TelemetryAlert } from 'src/generated-ts-client';

// List of all telemetry alerts received
export const telemetryAlertsListAtom = atom<TelemetryAlert[]>([]);
telemetryAlertsListAtom.debugLabel = 'Telemetry Alerts List Atom';

// Map of turbineId -> TelemetryAlert[] (latest first)
export const telemetryAlertsByTurbineAtom = atom<Record<string, TelemetryAlert[]>>({});
telemetryAlertsByTurbineAtom.debugLabel = 'Telemetry Alerts By Turbine Atom';

// Selected turbine id in the UI for alerts
export const selectedAlertTurbineAtom = atom<string | null>(null);
selectedAlertTurbineAtom.debugLabel = 'Selected Alert Turbine Atom';

// Derived atom: alerts for selected turbine
export const alertsForSelectedTurbineAtom = atom((get) => {
	const selected = get(selectedAlertTurbineAtom);
	const map = get(telemetryAlertsByTurbineAtom);
	if (!selected) return [] as TelemetryAlert[];
	return map[selected] ?? [];
});
alertsForSelectedTurbineAtom.debugLabel = 'Alerts For Selected Turbine Atom';

// Atom to store latest alert per turbine (single latest entry)
export const latestAlertByTurbineAtom = atom<Record<string, TelemetryAlert | null>>({});
latestAlertByTurbineAtom.debugLabel = 'Latest Alert By Turbine Atom';

// Helper action atom to push an alert into stores
export const pushTelemetryAlertAtom = atom(null, (get, set, alert: TelemetryAlert) => {
	// add to list
	const list = [...get(telemetryAlertsListAtom)];
	list.unshift(alert);
	set(telemetryAlertsListAtom, list.slice(0, 1000)); // cap for memory

	// add to map
	const map = { ...get(telemetryAlertsByTurbineAtom) } as Record<string, TelemetryAlert[]>;
	const key = (alert.turbineId ?? 'unknown') as string;
	map[key] = [alert, ...(map[key] ?? [])].slice(0, 500);
	set(telemetryAlertsByTurbineAtom, map);

	// update latest
	const latest = { ...get(latestAlertByTurbineAtom) } as Record<string, TelemetryAlert | null>;
	latest[key] = alert;
	set(latestAlertByTurbineAtom, latest);
});

