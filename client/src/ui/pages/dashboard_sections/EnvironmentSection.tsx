import React from 'react';
import LineChartComponent from '@ui/components/LineChartComponent';
import WindDirectionIndicator from '@ui/components/WindDirectionIndicator';
import { useAtom } from 'jotai';
import { telemetryForSelectedTurbineAtom } from '@core/atoms/telemetryAtoms';
import { TelemetryDto } from 'src/generated-ts-client';

export default function EnvironmentSection() {
  const [telemetry] = useAtom(telemetryForSelectedTurbineAtom);

  const ambientData = (telemetry ?? []).map((t) => ({ timestamp: t.timestamp, value: t.ambientTemperature }));
  const windData = (telemetry ?? []).map((t) => ({ timestamp: t.timestamp, value: t.windSpeed }));
  const latest = (telemetry ?? [])[0] as TelemetryDto | undefined;
  const windDir = latest?.windDirection ?? null;

  return (
    <section className="space-y-6 w-auto">
      <div className="flex items-center justify-between">
        <h3 className="text-xl font-semibold">Environment</h3>
      </div>

      <div className="section-content">
        <div className="rounded-lg border border-slate-200 bg-white p-4 shadow-sm dark:border-slate-700 dark:bg-slate-900/60">
          <LineChartComponent title="Ambient Temperature (°C)" height={280} color="#f97316" data={ambientData} />
        </div>

        <div className="rounded-lg border border-slate-200 bg-white p-4 shadow-sm dark:border-slate-700 dark:bg-slate-900/60">
          <LineChartComponent title="Wind Speed (m/s)" height={280} color="#0ea5e9" data={windData} />
        </div>

        <div className="rounded-lg border border-slate-200 bg-white p-4 shadow-sm dark:border-slate-700 dark:bg-slate-900/60 flex items-center justify-center">
          <WindDirectionIndicator degrees={windDir ?? undefined} size={180} />
        </div>
      </div>
    </section>
  );
}
