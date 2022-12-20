import './App.css';
import { BrowserRouter, Routes, Route } from 'react-router-dom'
import Dashboard from './views/Dashboard';

function App() {
  return (
    <div className="wrapper">
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Dashboard />} />
        </Routes>
      </BrowserRouter>
    </div>
  );
}

export default App;
