
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import LoginPage from './ui/pages/LoginPage';
import HomePage from './ui/pages/HomePage';
import { useSse } from './utils/useSse';
import Layout from './ui/pages/Layout';
import { useEffect } from 'react';
import { useAtom } from 'jotai';
import { authAtom } from '@core/atoms/authAtoms';
import { api } from '@utils/api';
import { User } from '@core/types/User';
import { authApi } from '@core/controllers/authApi';
import { StateleSSEClient } from 'statele-sse';
import { RealtimeClient } from './generated-ts-client';
import RealtimeComponent from '@ui/components/RealtimeComponent';
// import './App.css';



const router = createBrowserRouter([
  {
    path: '/',
    element: <Layout />,
    children: [
      {
        path: '/',
        element: <HomePage />,
      },
    ],
  },
  {
    path: '/login',
    element: <LoginPage />,
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
    {auth.status === "authenticated" && (<RealtimeComponent />)}
    <RouterProvider router={router} />
  </>;
}

export default App;
