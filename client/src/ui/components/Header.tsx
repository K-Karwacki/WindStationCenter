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
    <header className="sticky top-0 z-40 border-b border-sky-700 bg-sky-600 text-white backdrop-blur shadow-sm dark:bg-sky-800/95">
      <div className="mx-auto flex h-16 w-full max-w-6xl items-center justify-between p-9">
        <h1 className="font-semibold tracking-tight text-white md:text-lg lg:text-2xl text-4xl!">
          Wind Farm Control Center
        </h1>

        {auth.status === "unauthenticated" ? (
          <a
            href="/login"
            className="rounded-md bg-white px-4 py-2 text-sm font-medium text-sky-700 transition hover:bg-white/90 focus:outline-none focus:ring-2 focus:ring-white/30 block w-[5rem] text-center"
          >
            Login
          </a>
        ) : (
          <a
            onClick={handleLogout}
            className="rounded-md bg-white px-4 py-2 text-sm font-medium text-sky-700 transition hover:bg-white/90 focus:outline-none focus:ring-2 focus:ring-white/30 block cursor-pointer w-[5rem] text-center"
          >
            Logout
          </a>
        )}
      </div>
    </header>
  );
}