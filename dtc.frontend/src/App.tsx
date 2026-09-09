import { Routes, Route, Navigate } from "react-router";
import "./App.css";
import HomePage from "./pages/home/HomePage";
import { SettingsPage } from "./pages/settings/SettingsPage";
import { PlayerPage } from "./pages/player/PlayerPage";
import { DashboardPage } from "./pages/dashboard/DashboardPage";
import { TournamentPage } from "./pages/tournament/TournamentPage";
import { CreateBoardPage } from "./pages/tournament/CreateBoardPage";
import { CreateTournamentPage } from "./pages/tournament/CreateTournamentPage";
import { CreateLocationPage } from "./pages/location/CreateLocationPage";
import BoardQrPage from "./pages/board/BoardQrPage";
import BoardAccessPage from "./pages/board/BoardAccessPage";
import { ManageTournamentPage } from "./pages/tournament/ManageTournamentPage";

function App() {
  return (
    <>
      <Routes>
        <Route index element={<Navigate to="/dashboard" replace />} />
        <Route path="tournaments" element={<TournamentPage />} />
        <Route
          path="tournaments/:id/manage"
          element={<ManageTournamentPage />}
        />
        <Route path="settings" element={<SettingsPage />} />
        <Route path="players" element={<PlayerPage />} />
        <Route path="dashboard" element={<DashboardPage />} />
        <Route path="boards/create" element={<CreateBoardPage />} />
        <Route path="boards/:boardId" element={<BoardAccessPage />} />
        <Route path="boards/:boardId/qr" element={<BoardQrPage />} />
        <Route path="tournaments/create" element={<CreateTournamentPage />} />
        <Route path="location/create" element={<CreateLocationPage />} />
      </Routes>
    </>
  );
}

export default App;
