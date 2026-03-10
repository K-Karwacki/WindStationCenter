import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAtom } from 'jotai';
import { authAtom } from '@core/atoms/authAtoms';

interface ProtectedRouteProps {
  children: React.ReactElement;
}

export default function ProtectedRoute({ children }: ProtectedRouteProps) {
  const [auth] = useAtom(authAtom);
  const location = useLocation();

  // If auth status is still unknown, you can render null or a loader
  if (!auth || auth.status === undefined) return null;

  if (auth.status === 'authenticated') {
    return children;
  }

  // Redirect unauthenticated users to login and preserve location
  return <Navigate to="/login" replace state={{ from: location }} />;
}
