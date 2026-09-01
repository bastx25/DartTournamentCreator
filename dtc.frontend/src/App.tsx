import { Routes, Route } from "react-router";
import "./App.css";
import HomePage from "./pages/home/HomePage";
import { CreateTournamentPage } from "./pages/tournament/CreateTournamentPage";
import { SettingsPage } from "./pages/settings/SettingsPage";
import { PlayerPage } from "./pages/player/PlayerPage";
import { DashboardPage } from "./pages/dashboard/DashboardPage";

function App() {
  return (
    <>
      <Routes>
        <Route index element={<HomePage />} />
        <Route path="tournaments/create" element={<CreateTournamentPage />} />
        <Route path="settings" element={<SettingsPage />} />
        <Route path="players" element={<PlayerPage />} />
        <Route path="dashboard" element={<DashboardPage />} />
      </Routes>
    </>
  );
}

export default App;
