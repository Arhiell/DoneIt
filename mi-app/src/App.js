import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import RegistroPage from "./pages/RegistroPage";

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<LoginPage />} />
        <Route path="/registro" element={<RegistroPage />} />
        {/* Aquí puedes agregar más rutas según sea necesario */}

      </Routes>
    </Router>
  );
}

export default App;
