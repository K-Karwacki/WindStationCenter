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
    <header className="w-full sticky top-0 z-40">
      <div className="navbar bg-primary text-primary-content shadow-md">
        <div className="container mx-auto px-4">
          <div className="flex-1">
            <a className="normal-case text-xl">Wind Farm Control Center</a>
          </div>
          <div className="flex-none">
            {auth.status === "unauthenticated" ? (
              <a href="/login" className="btn btn-sm btn-ghost">Login</a>
            ) : (
              <button onClick={handleLogout} className="btn btn-sm btn-ghost">Logout</button>
            )}
          </div>
        </div>
      </div>
    </header>
  );
}