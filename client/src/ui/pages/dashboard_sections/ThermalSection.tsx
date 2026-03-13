import React from 'react';
import LineChartComponent from '@ui/components/LineChartComponent';
import { useAtom } from 'jotai';
import { telemetryForSelectedTurbineAtom } from '@core/atoms/telemetryAtoms';
import { TelemetryDto } from 'src/generated-ts-client';

export default function ThermalSection() {
  const [telemetry] = useAtom(telemetryForSelectedTurbineAtom);

  const generatorTempData = (telemetry ?? []).map((t) => ({ timestamp: t.timestamp, value: t.generatorTemp }));
  const gearboxTempData = (telemetry ?? []).map((t) => ({ timestamp: t.timestamp, value: t.gearboxTemp }));

  return (
    <section className="space-y-6">
      <div className="flex items-center justify-between">
        <h3 className="text-xl font-semibold">Thermal</h3>
      </div>

      <div className="section-content">
        <div className="rounded-lg border border-slate-200 bg-white p-4 shadow-sm dark:border-slate-700 dark:bg-slate-900/60">
          <LineChartComponent title="Generator Temperature (°C)" height={280} color="#f97316" data={generatorTempData} />
        </div>

        <div className="rounded-lg border border-slate-200 bg-white p-4 shadow-sm dark:border-slate-700 dark:bg-slate-900/60">
          <LineChartComponent title="Gearbox Temperature (°C)" height={280} color="#0ea5e9" data={gearboxTempData} />
        </div>
      </div>
    </section>
  );
}
