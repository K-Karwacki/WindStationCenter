import React from 'react';
import { Link } from 'react-router-dom';

export default function NotFound() {
  return (
    <div className="py-24">
      <div className="mx-auto max-w-3xl text-center">
        <h1 className="text-4xl font-bold">404 — Page not found</h1>
        <p className="mt-4 text-sm text-slate-600">We couldn't find the page you're looking for.</p>
        <div className="mt-6">
          <Link to="/" className="rounded-md bg-sky-600 px-4 py-2 text-sm font-medium text-white">
            Go home
          </Link>
        </div>
      </div>
    </div>
  );
}
