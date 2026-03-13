import React from 'react';

export interface BladePitchIndicatorProps {
  pitch?: number | null; // degrees
  size?: number;
}

export default function BladePitchIndicator({ pitch, size = 180 }: BladePitchIndicatorProps) {
  const value = typeof pitch === 'number' && !Number.isNaN(pitch) ? pitch : 0;
  // map pitch [0, 90] -> angle [-90, 90]
  const clamped = Math.max(0, Math.min(90, value));
  const angle = (clamped / 90) * 180 - 90;

  const svgSize = size;
  const centerX = svgSize / 2;
  const centerY = svgSize / 2;
  const needleLength = svgSize * 0.38;

  // ticks
  const ticks = [] as number[];
  for (let i = 0; i <= 9; i++) ticks.push(i * 10);

  return (
    <div className="card bg-base-100 shadow p-2 text-center">
      <div className="text-sm opacity-60 mb-2">Blade Pitch</div>
      <svg width={svgSize} height={svgSize} viewBox={`0 0 ${svgSize} ${svgSize}`}>
        <defs>
          <linearGradient id="pitchGrad" x1="0" x2="1">
            <stop offset="0%" stopColor="#34d399" />
            <stop offset="100%" stopColor="#10b981" />
          </linearGradient>
        </defs>

        {/* outer circle */}
        <circle cx={centerX} cy={centerY} r={svgSize * 0.45} fill="none" stroke="#e6eef3" strokeWidth={2} />

        {/* ticks around semicircle */}
        <g transform={`translate(${centerX}, ${centerY})`}>
          {ticks.map((t) => {
            const a = ((t / 90) * 180 - 90) * (Math.PI / 180);
            const rOuter = svgSize * 0.45;
            const rInner = svgSize * 0.38;
            const x1 = Math.cos(a) * rOuter;
            const y1 = Math.sin(a) * rOuter;
            const x2 = Math.cos(a) * rInner;
            const y2 = Math.sin(a) * rInner;
            return <line key={t} x1={x1} y1={y1} x2={x2} y2={y2} stroke="#cbd5e1" strokeWidth={2} />;
          })}
        </g>

        {/* needle */}
        <g transform={`translate(${centerX}, ${centerY}) rotate(${angle})`}>
          <line x1={0} y1={0} x2={0} y2={-needleLength} stroke="url(#pitchGrad)" strokeWidth={6} strokeLinecap="round" />
          <circle cx={0} cy={0} r={6} fill="#0ea5e9" />
        </g>
      </svg>

      <div className="mt-3 text-center">
        <div className="text-lg font-medium">{Math.round(value)}°</div>
        <div className="text-sm opacity-60">{clamped}°</div>
      </div>
    </div>
  );
}
