
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import LoginPage from './ui/pages/LoginPage';
import HomePage from './ui/pages/HomePage';
import ProtectedRoute from './ui/components/ProtectedRoute';
import Layout from './ui/pages/Layout';
import { useEffect } from 'react';
import { useAtom } from 'jotai';
import { authAtom } from '@core/atoms/authAtoms';
import { authApi } from '@core/controllers/authApi';
import { StateleSSEClient } from 'statele-sse';
import { RealtimeClient } from './generated-ts-client';
import RealtimeComponent from '@ui/components/RealtimeComponent';
import { Dashboard } from '@ui/pages/Dashboard';
import RedirectIfAuthenticated from './ui/components/RedirectIfAuthenticated';
import NotFound from './ui/pages/NotFound';
import { Navigate } from 'react-router-dom';
// import './App.css';


const router = createBrowserRouter([
  {
    path: '/',
    element: <Layout />,
    children: [
      // index -> redirect to dashboard
      {
        index: true,
        element: <Navigate to="/dashboard" replace />,
      },

      {
        path: '/dashboard',
        element: (
          <ProtectedRoute>
            <Dashboard />
          </ProtectedRoute>
        ),
      },

      // catch-all under layout
      {
        path: '*',
        element: <NotFound />,
      },
      
      {
        path: '/login',
        element: (
          <RedirectIfAuthenticated>
            <LoginPage />
          </RedirectIfAuthenticated>
        ),
      },
    ],
  },
]);

function App() {
  const [auth, setAuth] = useAtom(authAtom)

  useEffect(() => {
    const checkAuth = async () => {
      try {
        const user = await authApi.me();
        setAuth({ status: "authenticated", user: user });
      } catch {
        setAuth({ status: "unauthenticated" })
      }
    }

    checkAuth()
  }, []);
  
  return <>
    {auth.status === "authenticated" && (<RealtimeComponent sse={new StateleSSEClient("api/realtime/sse")} realtimeClient={new RealtimeClient()} />)}
    <RouterProvider router={router} />
  </>;
}

export default App;
