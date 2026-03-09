import { atom } from 'jotai';
import { Telemetry } from 'src/generated-ts-client';

// List of all telemetry points received
export const telemetryListAtom = atom<Telemetry[]>([]);
telemetryListAtom.debugLabel = 'Telemetry List Atom';

// Map of turbineId -> Telemetry[] (latest first)
export const telemetryByTurbineAtom = atom<Record<string, Telemetry[]>>({});
telemetryByTurbineAtom.debugLabel = 'Telemetry By Turbine Atom';

// Selected turbine id in the UI
export const selectedTurbineAtom = atom<string | null>(null);
selectedTurbineAtom.debugLabel = 'Selected Turbine Atom';

// Derived atom: telemetry for selected turbine
export const telemetryForSelectedTurbineAtom = atom((get) => {
  const selected = get(selectedTurbineAtom);
  const map = get(telemetryByTurbineAtom);
  if (!selected) return [] as Telemetry[];
  return map[selected] ?? [];
});
telemetryForSelectedTurbineAtom.debugLabel = 'Telemetry For Selected Turbine Atom';

// Atom to store latest telemetry per turbine (single latest entry)
export const latestTelemetryByTurbineAtom = atom<Record<string, Telemetry | null>>({});
latestTelemetryByTurbineAtom.debugLabel = 'Latest Telemetry By Turbine Atom';

// Helper action atom to push a telemetry point into stores
export const pushTelemetryAtom = atom(null, (get, set, telemetry: Telemetry) => {
  // add to list
  const list = [...get(telemetryListAtom)];
  list.unshift(telemetry);
  set(telemetryListAtom, list.slice(0, 1000)); // cap for memory

  // add to map
  const map = { ...get(telemetryByTurbineAtom) } as Record<string, Telemetry[]>;
  const key = telemetry.turbineId ?? 'unknown';
  map[key] = [telemetry, ...(map[key] ?? [])].slice(0, 500);
  set(telemetryByTurbineAtom, map);

  // update latest
  const latest = { ...get(latestTelemetryByTurbineAtom) } as Record<string, Telemetry | null>;
  latest[key] = telemetry;
  set(latestTelemetryByTurbineAtom, latest);
});
