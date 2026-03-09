import { authAtom } from '@core/atoms/authAtoms';
import { useAtom } from 'jotai';
import { authApi } from '@core/controllers/authApi';

export default function Header() {
  const [auth, setAuth] = useAtom(authAtom);

  const handleLogout = async () => {
    await authApi.logout()
      .then(() => {
        setAuth({ status: "unauthenticated" });
      })
      .catch((err: any) => {
        console.error('Logout failed:', err);
        alert(err.message || 'Logout failed'); 
      });
  };

  return (
    <header style={{ padding: 16, backgroundColor: '#282c34', color: 'white' }}>
      <h1>Wind Station Controll Center</h1>
      {auth.status === "unauthenticated" ? <a href="/login">Login</a> : <button onClick={handleLogout}>Logout</button>}
    </header>
  );
}