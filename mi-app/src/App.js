import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import LoginPage from './pages/LoginPage';
import RegistroPage from "./pages/RegistroPage";
import HomePage from './pages/HomePage';
import 'bootstrap/dist/css/bootstrap.min.css';
import { Navigate } from 'react-router-dom';

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Navigate to="/login" />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/registro" element={<RegistroPage />} />
        <Route path="/dashboard/*" element={<HomePage />} />
      </Routes>
    </Router>
  );
}


export default App;
// para levantar el servidor de desarrollo, ejecuta:
// cd /b/repogithub/DoneIt/mi-app
// npm start