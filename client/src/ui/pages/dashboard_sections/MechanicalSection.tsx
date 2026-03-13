import React from 'react';
import LineChartComponent from '@ui/components/LineChartComponent';
import { useAtom } from 'jotai';
import { telemetryForSelectedTurbineAtom } from '@core/atoms/telemetryAtoms';
import { TelemetryDto } from 'src/generated-ts-client';
import WindDirectionIndicator from '@ui/components/WindDirectionIndicator';
import BladePitchIndicator from '@ui/components/BladePitchIndicator';
import PowerOutputIndicator from '@ui/components/PowerOutputIndicator';

export default function MechanicalSection() {
  const [telemetry] = useAtom(telemetryForSelectedTurbineAtom);

  const rotorSpeedData = (telemetry ?? []).map((t) => ({ timestamp: t.timestamp, value: t.rotorSpeed }));
  const vibrationData = (telemetry ?? []).map((t) => ({ timestamp: t.timestamp, value: t.vibration }));
  const powerData = (telemetry ?? []);
  const latest = (telemetry ?? [])[0] as TelemetryDto | undefined;
  const bladePitch = latest?.bladePitch ?? null;
  const nacelleDir = latest?.nacelleDirection ?? null;

  return (
    <section className="space-y-6">
      <div className="flex items-center justify-between">
        <h3 className="text-xl font-semibold">Mechanical</h3>
      </div>

      <div className="section-content">
        <div className="rounded-lg border border-slate-200 bg-white p-4 shadow-sm dark:border-slate-700 dark:bg-slate-900/60">
          <LineChartComponent title="Rotor Speed (RPM)" height={280} color="#f97316" data={rotorSpeedData} />
        </div>

        <div className="rounded-lg border border-slate-200 bg-white p-4 shadow-sm dark:border-slate-700 dark:bg-slate-900/60">
          <LineChartComponent title="Vibration (m/s²)" height={280} color="#0ea5e9" data={vibrationData} />
        </div>
        <div className="rounded-lg border border-slate-200 bg-white p-4 shadow-sm dark:border-slate-700 dark:bg-slate-900/60 flex items-center justify-center">
          <WindDirectionIndicator degrees={nacelleDir ?? undefined} size={180} title="Nacelle Direction" />
        </div>

        <div className="rounded-lg border border-slate-200 bg-white p-4 shadow-sm dark:border-slate-700 dark:bg-slate-900/60 flex items-center justify-center">
          <BladePitchIndicator pitch={bladePitch ?? undefined} size={160} />
        </div>

        <div className="rounded-lg border border-slate-200 bg-white p-4 shadow-sm dark:border-slate-700 dark:bg-slate-900/60 h-auto flex items-center justify-center">
          <PowerOutputIndicator latest={powerData[0]?.powerOutput ?? null} history={powerData} />
        </div>

      </div>
    </section>
  );
}
