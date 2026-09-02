import { Routes, Route } from "react-router";
import "./App.css";
import HomePage from "./pages/home/HomePage";
import { SettingsPage } from "./pages/settings/SettingsPage";
import { PlayerPage } from "./pages/player/PlayerPage";
import { DashboardPage } from "./pages/dashboard/DashboardPage";
import { TournamentPage } from "./pages/tournament/TournamentPage";
import { CreateBoardPage } from "./pages/tournament/CreateBoardPage";
import { CreateTournamentPage } from "./pages/tournament/CreateTournamentPage";
import { CreateLocationPage } from "./pages/location/CreateLocationPage";
import { TournamentManagePage } from "./pages/tournament/TournamentManagePage";

function App() {
  return (
    <>
      <Routes>
        <Route index element={<HomePage />} />
        <Route path="tournaments" element={<TournamentPage />} />
        <Route
          path="tournaments/:id/manage"
          element={<TournamentManagePage />}
        />
        <Route path="settings" element={<SettingsPage />} />
        <Route path="players" element={<PlayerPage />} />
        <Route path="dashboard" element={<DashboardPage />} />
        <Route path="boards/create" element={<CreateBoardPage />} />
        <Route path="tournaments/create" element={<CreateTournamentPage />} />
        <Route path="location/create" element={<CreateLocationPage />} />
      </Routes>
    </>
  );
}

export default App;
