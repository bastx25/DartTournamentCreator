import { Routes, Route } from "react-router";
import "./App.css";
import HomePage from "./pages/home/HomePage";
import { CreateTournamentPage } from "./pages/tournament/CreateTournamentPage";

function App() {
  return (
    <>
      <Routes>
        <Route index element={<HomePage />} />
        <Route path="tournaments/create" element={<CreateTournamentPage />} />
      </Routes>
    </>
  );
}

export default App;
