import React from 'react';

export interface WindDirectionIndicatorProps {
  degrees?: number | null;
  size?: number; // px
  title?: string;
}

function degToCardinal(deg: number) {
  const directions = ['N', 'NNE', 'NE', 'ENE', 'E', 'ESE', 'SE', 'SSE', 'S', 'SSW', 'SW', 'WSW', 'W', 'WNW', 'NW', 'NNW'];
  const index = Math.round(((deg % 360) / 22.5)) % 16;
  return directions[index];
}

export default function WindDirectionIndicator({ degrees, size = 200, title = "Wind direction" }: WindDirectionIndicatorProps) {
  const deg = typeof degrees === 'number' && !Number.isNaN(degrees) ? ((degrees % 360) + 360) % 360 : 0;
  const label = degToCardinal(deg);

  const svgSize = size;
  const center = svgSize / 2;
  const arrowLength = svgSize * 0.35;

  return (
    <div className="card bg-base-100 shadow p-2 text-center">
      {title && <div className="text-sm opacity-60 mb-2">{title}</div>}
      <svg width={svgSize} height={svgSize} viewBox={`0 0 ${svgSize} ${svgSize}`}>
        <defs>
          <linearGradient id="arrowGrad" x1="0" x2="1">
            <stop offset="0%" stopColor="#06b6d4" />
            <stop offset="100%" stopColor="#0ea5e9" />
          </linearGradient>
        </defs>

        {/* Compass circle */}
        <circle cx={center} cy={center} r={svgSize * 0.45} fill="none" stroke="#e2e8f0" strokeWidth={2} />

        {/* Cardinal markers */}
        <g fontSize={12} textAnchor="middle" fill="#334155" style={{ fontFamily: 'Inter, system-ui, -apple-system, "Segoe UI", Roboto, "Helvetica Neue", Arial' }}>
          <text x={center} y={center - svgSize * 0.4 + 14}>N</text>
          <text x={center + svgSize * 0.4 - 8} y={center + 4}>E</text>
          <text x={center} y={center + svgSize * 0.4 - 4}>S</text>
          <text x={center - svgSize * 0.4 + 8} y={center + 4}>W</text>
        </g>

        {/* Rotating arrow (rotate around center) */}
        <g transform={`rotate(${deg} ${center} ${center})`}>
          <line x1={center} y1={center} x2={center} y2={center - arrowLength} stroke="url(#arrowGrad)" strokeWidth={6} strokeLinecap="round" />
          <polygon
            points={`${center - 8},${center - arrowLength + 18} ${center + 8},${center - arrowLength + 18} ${center},${center - arrowLength - 8}`}
            fill="#0ea5e9"
          />
        </g>
      </svg>

      <div className="mt-3 text-center">
        <div className="text-lg font-medium">{Math.round(deg)}°</div>
        <div className="text-sm opacity-60">{label}</div>
      </div>
    </div>
  );
}
