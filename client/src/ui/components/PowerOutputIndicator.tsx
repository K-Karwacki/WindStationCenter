import React from 'react';
import { TelemetryDto } from 'src/generated-ts-client';

export interface PowerOutputIndicatorProps {
  latest?: number | null;
  history?: TelemetryDto[];
  size?: { width: number; height: number };
}

function buildSparkPath(values: number[], width: number, height: number) {
  if (!values.length) return '';
  const max = Math.max(...values);
  const min = Math.min(...values);
  const range = max - min || 1;
  return values
    .map((v, i) => {
      const x = (i / (values.length - 1 || 1)) * width;
      const y = height - ((v - min) / range) * height;
      return `${x},${y}`;
    })
    .join(' ');
}

export default function PowerOutputIndicator({ latest, history = [], size = { width: 220, height: 48 } }: PowerOutputIndicatorProps) {
  const values = history.map((h) => (h.powerOutput ?? 0));
  const sparkPoints = buildSparkPath(values, size.width, size.height);
  const display = typeof latest === 'number' && !Number.isNaN(latest) ? `${Math.round(latest)} kW` : '—';

  return (
    <div className="card bg-base-100 shadow p-3">
      <div className="card-body p-2">
        <h3 className="card-title text-sm">Power Output</h3>
        <div className="flex items-center gap-4 w-full">
          <div className="flex-1">
            <div className="text-2xl font-bold">{display}</div>
            <div className="text-xs opacity-60">Latest</div>
          </div>
          <div className="w-[240px]">
            <svg width={size.width} height={size.height} viewBox={`0 0 ${size.width} ${size.height}`}>
              <polyline
                fill="none"
                stroke="#0ea5e9"
                strokeWidth={2}
                strokeLinejoin="round"
                strokeLinecap="round"
                points={sparkPoints}
              />
            </svg>
          </div>
        </div>
      </div>
    </div>
  );
}
