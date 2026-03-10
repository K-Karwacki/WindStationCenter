export default function HomePage() {
  return (
    <section className="w-full bg-gradient-to-br from-sky-50 via-white to-indigo-50 py-10 dark:from-slate-950 dark:via-slate-900 dark:to-indigo-950">
      <div className="mx-auto w-full max-w-6xl px-4 sm:px-6 lg:px-8">
        <div className="rounded-2xl border border-slate-200 bg-white p-8 shadow-sm dark:border-slate-800 dark:bg-slate-900">
          <span className="inline-flex items-center rounded-full bg-sky-100 px-3 py-1 text-xs font-semibold text-sky-700 dark:bg-sky-900/40 dark:text-sky-300">
            Overview
          </span>

          <h2 className="mt-4 text-3xl font-bold tracking-tight text-slate-900 dark:text-slate-100">
            Home Page
          </h2>

          <p className="mt-3 max-w-2xl text-sm leading-6 text-slate-600 dark:text-slate-300">
            Welcome to the Wind Station Control Center. This dashboard gives you quick access to live telemetry,
            alerts, and system health at a glance.
          </p>

          <div className="mt-8 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            <div className="rounded-xl border border-slate-200 bg-slate-50 p-4 dark:border-slate-700 dark:bg-slate-800/60">
              <p className="text-xs font-medium uppercase tracking-wide text-slate-500 dark:text-slate-400">Telemetry</p>
              <p className="mt-2 text-sm font-medium text-slate-800 dark:text-slate-200">Track real-time wind and sensor data.</p>
            </div>

            <div className="rounded-xl border border-slate-200 bg-slate-50 p-4 dark:border-slate-700 dark:bg-slate-800/60">
              <p className="text-xs font-medium uppercase tracking-wide text-slate-500 dark:text-slate-400">Alerts</p>
              <p className="mt-2 text-sm font-medium text-slate-800 dark:text-slate-200">Stay informed on critical station events.</p>
            </div>

            <div className="rounded-xl border border-slate-200 bg-slate-50 p-4 dark:border-slate-700 dark:bg-slate-800/60">
              <p className="text-xs font-medium uppercase tracking-wide text-slate-500 dark:text-slate-400">Status</p>
              <p className="mt-2 text-sm font-medium text-slate-800 dark:text-slate-200">Monitor system connectivity and uptime.</p>
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}