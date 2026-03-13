import React, { useEffect, useState } from 'react';
import { useAtom, useSetAtom } from 'jotai';
import { selectedTurbineAtom } from '@core/atoms/telemetryAtoms';
import { turbineControlStateAtom, toggleTurbineRunningAtom, setReportIntervalAtom, setBladePitchAtom } from '@core/atoms/turbineControlAtoms';
import { latestTelemetryByTurbineAtom } from '@core/atoms/telemetryAtoms';
import { RealtimeClient } from '../../generated-ts-client';
import { telemetryAlertsByTurbineAtom } from '@core/atoms/telemetryAletrsAtoms';

export default function TurbineControls() {
  const [selected] = useAtom(selectedTurbineAtom);
  const [controls] = useAtom(turbineControlStateAtom);
  const toggleRunning = useSetAtom(toggleTurbineRunningAtom);
  const setIntervalAtom = useSetAtom(setReportIntervalAtom);
  const setPitch = useSetAtom(setBladePitchAtom);

  const state = selected ? controls[selected] ?? {} : {};
  const running = !!state.running;
  const currentInterval = state.reportIntervalMs ?? 5000;
  const currentPitch = state.bladePitch ?? 0;
  const [latestMap] = useAtom(latestTelemetryByTurbineAtom);
  const [latestAlerts] = useAtom(telemetryAlertsByTurbineAtom);
  const latestTelemetry = selected ? (latestMap[selected] ?? null) : null;

  const [intervalInput, setIntervalInput] = useState<number>(currentInterval);
  const [pitchInput, setPitchInput] = useState<number>(currentPitch);
  const [busy, setBusy] = useState(false);


  const realtimeClient = new RealtimeClient();

  // keep inputs in sync when selection or external telemetry changes
  useEffect(() => {
    setIntervalInput(currentInterval);
    if (state.bladePitch !== undefined) {
      setPitchInput(currentPitch);
    } else if (latestTelemetry?.bladePitch !== undefined && latestTelemetry?.bladePitch !== null) {
      setPitchInput(latestTelemetry.bladePitch as number);
    } else {
      setPitchInput(0);
    }
  }, [latestTelemetry]);

  const alertText = (() => {
    if(latestAlerts[selected!]?.length) {
      latestAlerts[selected!].forEach(alert => {
        if(alert) {
          return alert.message;
        }
      });
    }
    return null;
  })();

  if (!selected) {
    return (
      <div className="card bg-base-100 shadow p-4 mb-4">
        <div className="text-sm text-slate-600">Select a turbine to show controls.</div>
      </div>
    );
  }

  const onToggle = async (e: React.MouseEvent<HTMLButtonElement>) => {
    // Capture the button element synchronously to avoid React's event pooling
    const btn = e.currentTarget;
    setBusy(true);
    try {
        btn.classList.add('btn-disabled'); // prevent multiple clicks while processing
        btn.innerText = "Processing...";
        if(running) {
          await realtimeClient.sendStopCommandToTheTurbine(selected, "manualStop");
        } else {
          await realtimeClient.sendStartCommandToTheTurbine(selected);
        }
      toggleRunning(selected);
    } finally {
      setBusy(false);
      btn.classList.remove('btn-disabled');
      btn.innerText = running ? "Stop" : "Start";
    }
  };

  const onSetInterval = () => {
    setIntervalAtom({ turbineId: selected, intervalMs: Math.max(1000, Math.round(intervalInput)) });
    // realtimeClient.sendStopCommandToTheTurbine(selected, "reportIntervalChange");
  };

  const onSetPitch = async (e: React.MouseEvent<HTMLButtonElement>) => {
    const px = Math.max(0, Math.min(90, Math.round(pitchInput)));
    setPitch({ turbineId: selected, pitch: px });
    await realtimeClient.sendSetBladePitchCommandToTheTurbine(selected, px);
  };

  return (
    <div className="section-content">
      <div className="flex items-center justify-between mb-3">
        <div>
          <div className="text-lg font-semibold">Controls</div>
          <div className="text-sm text-slate-500">Turbine: {selected}</div>
        </div>
        <button
          type="button"
          onClick={(e) => onToggle(e)}
          disabled={busy}
          className={`${latestTelemetry?.status === 'running' ? 'btn-error' : 'btn-success'} btn`}
        >
          {(latestTelemetry?.status === 'running' ? 'Stop' : 'Start')}
        </button>
      </div>

      <div className="section-content">
        <div>
          <label className="text-sm text-slate-500 block mb-1">Report Interval (ms)</label>
          <div className="flex items-center gap-2">
            <input type="number" onChange={(e) => setIntervalInput(Number(e.target.value))} className="input input-bordered w-full" />
            <button onClick={onSetInterval} className="btn btn-sm btn-primary">Set</button>
          </div>
        </div>

        <div>
          <label className="text-sm text-slate-500 block mb-1">Blade Pitch (°)</label>
          <div className="flex items-center gap-2">
            <input type="range" min={0} max={90} onChange={(e) => setPitchInput(Number(e.target.value))} className="range range-primary w-full" />
            <button onClick={onSetPitch} className="btn btn-sm btn-primary">Set</button>
          </div>
          <div className="text-sm text-slate-500 mt-1">{pitchInput}°</div>
        </div>

        <div>
          <label className="text-sm text-slate-500 block mb-1">Alert</label>
          {latestTelemetry ? (
            <div>
              {alertText ? (
                <div className="alert alert-error shadow-sm">
                  <div>
                    <span>{alertText}</span>
                  </div>
                </div>
              ) : (
                <div className="alert shadow-sm">
                  <div>No active alerts</div>
                </div>
              )}
            </div>
          ) : (
            <div className="text-sm text-slate-500">No telemetry available</div>
          )}
        </div>

        <div>
          <label className="text-sm text-slate-500 block mb-1">State</label>
          <div className="text-sm text-slate-700">Telemetry status: {latestTelemetry?.status ?? '—'}</div>
          <div className="text-sm text-slate-700">Blade Pitch: {state.bladePitch ?? latestTelemetry?.bladePitch ?? '—'}°</div>
          <div className="text-sm text-slate-700">Rotor Speed: {latestTelemetry?.rotorSpeed ?? '—'}</div>
          <div className="text-sm text-slate-700">Power: {latestTelemetry?.powerOutput ?? '—'}</div>
        </div>
      </div>
    </div>
  );
}
