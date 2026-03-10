import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAtom } from 'jotai';
import { authAtom } from '@core/atoms/authAtoms';

interface Props {
  children: React.ReactElement;
}

export default function RedirectIfAuthenticated({ children }: Props) {
  const [auth] = useAtom(authAtom);
  const location = useLocation();

  if (!auth || auth.status === undefined) return null;

  if (auth.status === 'authenticated') {
    // If user is already authenticated, send them to the app root (or where they came from)
    const from = (location.state as any)?.from?.pathname ?? '/';
    return <Navigate to={from} replace />;
  }

  return children;
}
