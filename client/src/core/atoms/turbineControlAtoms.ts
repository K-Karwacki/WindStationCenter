import { atom } from 'jotai';

export interface TurbineControlState {
  running?: boolean;
  reportIntervalMs?: number;
  bladePitch?: number; // degrees
}

// Map of turbineId -> control state
export const turbineControlStateAtom = atom<Record<string, TurbineControlState>>({});

turbineControlStateAtom.debugLabel = 'Turbine Control State Atom';

// Action: start/stop turbine (mocked here; wire to API if available)
export const toggleTurbineRunningAtom = atom(null, (get, set, turbineId?: string) => {
  if (!turbineId) return;
  const map = { ...get(turbineControlStateAtom) } as Record<string, TurbineControlState>;
  const cur = map[turbineId] ?? {};
  const next = { ...cur, running: !cur.running };
  map[turbineId] = next;
  set(turbineControlStateAtom, map);
  console.log(`Turbine ${turbineId} ${next.running ? 'start' : 'stop'} requested (mock)`);
});

// Action: set report interval
export const setReportIntervalAtom = atom(null, (get, set, payload: { turbineId: string; intervalMs: number }) => {
  if (!payload?.turbineId) return;
  const { turbineId, intervalMs } = payload;
  const map = { ...get(turbineControlStateAtom) } as Record<string, TurbineControlState>;
  const cur = map[turbineId] ?? {};
  map[turbineId] = { ...cur, reportIntervalMs: intervalMs };
  set(turbineControlStateAtom, map);
  console.log(`Set report interval ${intervalMs}ms for turbine ${turbineId} (mock)`);
});

// Action: set blade pitch
export const setBladePitchAtom = atom(null, (get, set, payload: { turbineId: string; pitch: number }) => {
  if (!payload?.turbineId) return;
  const { turbineId, pitch } = payload;
  const map = { ...get(turbineControlStateAtom) } as Record<string, TurbineControlState>;
  const cur = map[turbineId] ?? {};
  map[turbineId] = { ...cur, bladePitch: pitch };
  set(turbineControlStateAtom, map);
  console.log(`Set blade pitch ${pitch}° for turbine ${turbineId} (mock)`);
});
