import React, { useState, useEffect } from "react";
import { useSse } from "@utils/useSse";
import { authAtom } from "@core/atoms/authAtoms";
import { useAtom } from "jotai";
import { api } from "@utils/api";

export default function HomePage() {

  return (
    <div>
      <h2>Home Page</h2>
      <p>Welcome to the Wind Station Controll Center! This is the home page.</p>
    </div>
  );
} 