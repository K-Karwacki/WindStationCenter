import { atom } from 'jotai';
import { GLOBAL_TELEMETRY_LIMIT, PER_TURBINE_LIMIT } from './telemetryAtoms';

export interface AutoPruneConfig {
  enabled: boolean;
  intervalMs: number; // how often to run pruning
  perTurbineLimit: number; // override per-turbine limit when pruning
  globalLimit: number; // override global limit when pruning
}

// default: disabled. enable in UI or set to true here if desired.
export const autoPruneConfigAtom = atom<AutoPruneConfig>({
  enabled: true,
  intervalMs: 30_000,
  perTurbineLimit: PER_TURBINE_LIMIT,
  globalLimit: GLOBAL_TELEMETRY_LIMIT,
});

autoPruneConfigAtom.debugLabel = 'Auto Prune Config Atom';
