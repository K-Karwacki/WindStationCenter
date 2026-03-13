import { atom } from 'jotai';
import { TelemetryDto } from 'src/generated-ts-client';

// memory limits (tweak these to reduce memory usage)
export const GLOBAL_TELEMETRY_LIMIT = 1000;
export const PER_TURBINE_LIMIT = 500;

// List of all telemetry points received (latest first)
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

// Helper to parse timestamp safely (returns epoch millis)
const tsToMillis = (t?: string) => {
  const m = t ? Date.parse(t) : NaN;
  return Number.isNaN(m) ? 0 : m;
};

// Rebuilds the global telemetry list from the per-turbine map and enforces global limit
function rebuildTelemetryListFromMap(map: Record<string, TelemetryDto[]>, globalLimit: number) {
  const all = Object.values(map).flat();
  all.sort((a, b) => tsToMillis(b.timestamp) - tsToMillis(a.timestamp));
  return all.slice(0, globalLimit);
}

// Helper action atom to push a telemetry point into stores
export const pushTelemetryAtom = atom(null, (get, set, telemetry: TelemetryDto) => {
  // add to per-turbine map
  const map = { ...get(telemetryByTurbineAtom) } as Record<string, TelemetryDto[]>;
  const key = telemetry.turbineId ?? 'unknown';
  map[key] = [telemetry, ...(map[key] ?? [])].slice(0, PER_TURBINE_LIMIT);
  set(telemetryByTurbineAtom, map);

  // rebuild and cap global list
  const newGlobal = rebuildTelemetryListFromMap(map, GLOBAL_TELEMETRY_LIMIT);
  set(telemetryListAtom, newGlobal);

  // update latest
  const latest = { ...get(latestTelemetryByTurbineAtom) } as Record<string, TelemetryDto | null>;
  latest[key] = telemetry;
  set(latestTelemetryByTurbineAtom, latest);
});

// Action to prune telemetry storage. Options:
// { keepOnlySelected?: boolean, perTurbineLimit?: number, globalLimit?: number, removeEmptyTurbines?: boolean }
export interface PruneOptions {
  keepOnlySelected?: boolean;
  perTurbineLimit?: number;
  globalLimit?: number;
  removeEmptyTurbines?: boolean;
}

export const pruneTelemetryAtom = atom(null, (get, set, opts?: PruneOptions) => {
  const options: PruneOptions = opts ?? {};
  const perLimit = options.perTurbineLimit ?? PER_TURBINE_LIMIT;
  const globalLimit = options.globalLimit ?? GLOBAL_TELEMETRY_LIMIT;
  const removeEmpty = !!options.removeEmptyTurbines;

  const selected = get(selectedTurbineAtom);
  const srcMap = { ...get(telemetryByTurbineAtom) } as Record<string, TelemetryDto[]>;
  let newMap: Record<string, TelemetryDto[]> = {};

  if (options.keepOnlySelected && selected) {
    newMap[selected] = (srcMap[selected] ?? []).slice(0, perLimit);
  } else {
    for (const k of Object.keys(srcMap)) {
      const arr = (srcMap[k] ?? []).slice(0, perLimit);
      if (arr.length > 0 || !removeEmpty) newMap[k] = arr;
    }
  }

  set(telemetryByTurbineAtom, newMap);

  // rebuild global list and latest map
  const newGlobal = rebuildTelemetryListFromMap(newMap, globalLimit);
  set(telemetryListAtom, newGlobal);

  const latest: Record<string, TelemetryDto | null> = {};
  for (const k of Object.keys(newMap)) {
    latest[k] = newMap[k].length > 0 ? newMap[k][0] : null;
  }
  set(latestTelemetryByTurbineAtom, latest);
});

// Action to clear telemetry either globally or for a specific turbine
export const clearTelemetryAtom = atom(null, (get, set, turbineId?: string) => {
  if (turbineId) {
    const map = { ...get(telemetryByTurbineAtom) } as Record<string, TelemetryDto[]>;
    delete map[turbineId];
    set(telemetryByTurbineAtom, map);
    set(telemetryListAtom, rebuildTelemetryListFromMap(map, GLOBAL_TELEMETRY_LIMIT));
    const latest = { ...get(latestTelemetryByTurbineAtom) } as Record<string, TelemetryDto | null>;
    delete latest[turbineId];
    set(latestTelemetryByTurbineAtom, latest);
    return;
  }

  // clear everything
  set(telemetryByTurbineAtom, {});
  set(telemetryListAtom, []);
  set(latestTelemetryByTurbineAtom, {});
});

// Set telemetry in bulk (clears existing and writes incoming list efficiently)
export const setTelemetryBulkAtom = atom(null, (get, set, data: TelemetryDto[]) => {
  if (!Array.isArray(data) || data.length === 0) {
    // clearing when no data
    set(telemetryByTurbineAtom, {});
    set(telemetryListAtom, []);
    set(latestTelemetryByTurbineAtom, {});
    return;
  }

  const perLimit = PER_TURBINE_LIMIT;
  const globalLimit = GLOBAL_TELEMETRY_LIMIT;

  const map: Record<string, TelemetryDto[]> = {};
  for (const t of data) {
    const key = t.turbineId ?? 'unknown';
    if (!map[key]) map[key] = [];
    map[key].push(t);
  }

  // sort each turbine's array by timestamp desc and cap
  for (const k of Object.keys(map)) {
    map[k].sort((a, b) => tsToMillis(b.timestamp) - tsToMillis(a.timestamp));
    map[k] = map[k].slice(0, perLimit);
  }

  set(telemetryByTurbineAtom, map);

  const newGlobal = rebuildTelemetryListFromMap(map, globalLimit);
  set(telemetryListAtom, newGlobal);

  const latest: Record<string, TelemetryDto | null> = {};
  for (const k of Object.keys(map)) {
    latest[k] = map[k].length > 0 ? map[k][0] : null;
  }
  set(latestTelemetryByTurbineAtom, latest);
});
