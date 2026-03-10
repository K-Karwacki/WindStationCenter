import React, { useMemo } from 'react';
import { ResponsiveContainer, LineChart, CartesianGrid, XAxis, YAxis, Tooltip, Line } from 'recharts';

type DataPoint = {
  // accepted time keys: time, timestamp, timeStamp
  time?: string | number | Date;
  timestamp?: string | number | Date;
  timeStamp?: string | number | Date;
  value?: number | null | string;
};

interface LineChartProps {
  data?: DataPoint[];
  title?: string;
  height?: number; // pixels
  color?: string;
}

function parseToMs(raw: string | number | Date | undefined) {
  if (raw == null) return NaN;
  if (raw instanceof Date) return raw.getTime();
  const n = Number(raw);
  if (!Number.isNaN(n)) {
    // if looks like seconds (<= 1e12) treat as seconds
    return n > 1e12 ? n : n * 1000;
  }
  // try parsing date string (ISO)
  const d = new Date(String(raw));
  if (!isNaN(d.getTime())) return d.getTime();
  return NaN;
}

export default function LineChartComponent({ data = [], title, height = 300, color = '#007bff' }: LineChartProps) {
  const normalized = useMemo(() => {
    const items = (data || [])
      .map((p, i) => {
        const rawTime = p.time ?? p.timestamp ?? p.timeStamp ?? i;
        const epochMs = parseToMs(rawTime as any);
        const value = p.value == null ? null : Number(p.value);
        return { epochMs, value };
      })
      .filter((d) => typeof d.value === 'number' && Number.isFinite(d.value) && Number.isFinite(d.epochMs));

    // sort by time ascending
    items.sort((a, b) => a.epochMs - b.epochMs);

    // map to Recharts-friendly shape
    return items.map((it) => ({ time: it.epochMs, value: it.value }));
  }, [data]);

  return (
    <div>
      {title && <h3 className="text-lg font-medium mb-2">{title}</h3>}
      <div style={{ width: '100%', height }}>
        {normalized.length === 0 ? (
          <div className="flex h-full items-center justify-center text-sm text-slate-500">No data</div>
        ) : (
          <ResponsiveContainer width="100%" height="100%">
            <LineChart data={normalized}>
              <CartesianGrid stroke="#e6e6e6" strokeDasharray="3 3" />
              <XAxis
                dataKey="time"
                type="number"
                domain={["dataMin", "dataMax"]}
                tick={{ fill: '#374151' }}
                tickFormatter={(val) => new Date(val).toLocaleTimeString()}
              />
              <YAxis tick={{ fill: '#374151' }} />
              <Tooltip labelFormatter={(val) => new Date(Number(val)).toLocaleString()} />
              <Line type="monotone" dataKey="value" stroke={color} strokeWidth={2} dot={false} />
            </LineChart>
          </ResponsiveContainer>
        )}
      </div>
    </div>
  );
}
