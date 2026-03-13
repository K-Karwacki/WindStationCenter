import React from 'react';
import { Outlet } from 'react-router-dom';
import Header from '../components/Header';

const Layout: React.FC = () => {
  return (
    <div className="min-h-screen w-screen overflow-x-hidden bg-transparent">
      <Header />

      <main className="pt-16">
        <div className="mx-auto px-4 sm:px-6 lg:px-8 flex justify-center w-full">
          <Outlet />
        </div>
      </main>
    </div>
  );
};

export default Layout;
