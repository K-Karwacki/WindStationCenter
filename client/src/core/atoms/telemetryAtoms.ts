import { atom } from 'jotai';
import { TelemetryDto } from 'src/generated-ts-client';

// List of all telemetry points received
export const telemetryListAtom = atom<TelemetryDto[]>([]);
telemetryListAtom.debugLabel = 'Telemetry List Atom';

// Map of turbineId -> Telemetry[] (latest first)
export const telemetryByTurbineAtom = atom<Record<string, TelemetryDto[]>>({});
telemetryByTurbineAtom.debugLabel = 'Telemetry By Turbine Atom';

// Selected turbine id in the UI
export const selectedTurbineAtom = atom<string | null>(null);
selectedTurbineAtom.debugLabel = 'Selected Turbine Atom';

// Derived atom: telemetry for selected turbine
export const telemetryForSelectedTurbineAtom = atom((get) => {
  const selected = get(selectedTurbineAtom);
  const map = get(telemetryByTurbineAtom);
  if (!selected) return [] as TelemetryDto[];
  return map[selected] ?? [];
});
telemetryForSelectedTurbineAtom.debugLabel = 'Telemetry For Selected Turbine Atom';

// Atom to store latest telemetry per turbine (single latest entry)
export const latestTelemetryByTurbineAtom = atom<Record<string, TelemetryDto | null>>({});
latestTelemetryByTurbineAtom.debugLabel = 'Latest Telemetry By Turbine Atom';

// Helper action atom to push a telemetry point into stores
export const pushTelemetryAtom = atom(null, (get, set, telemetry: TelemetryDto) => {
  // add to list
  const list = [...get(telemetryListAtom)];
  list.unshift(telemetry);
  set(telemetryListAtom, list.slice(0, 1000)); // cap for memory

  // add to map
  const map = { ...get(telemetryByTurbineAtom) } as Record<string, TelemetryDto[]>;
  const key = telemetry.turbineId ?? 'unknown';
  map[key] = [telemetry, ...(map[key] ?? [])].slice(0, 500);
  set(telemetryByTurbineAtom, map);

  // update latest
  const latest = { ...get(latestTelemetryByTurbineAtom) } as Record<string, TelemetryDto | null>;
  latest[key] = telemetry;
  set(latestTelemetryByTurbineAtom, latest);
});
