import { Routes, Route, Navigate } from "react-router";
import "./App.css";
import { AuthProvider } from "./context/AuthContext";
import { ProtectedRoute } from "./components/ProtectedRoute";
import { LoginPage } from "./pages/auth/LoginPage";
import { RegisterPage } from "./pages/auth/RegisterPage";
import { SettingsPage } from "./pages/settings/SettingsPage";
import { PlayerPage } from "./pages/player/PlayerPage";
import { DashboardPage } from "./pages/dashboard/DashboardPage";
import { TournamentPage } from "./pages/tournament/TournamentPage";
import { CreateTournamentPage } from "./pages/tournament/CreateTournamentPage";
import BoardQrPage from "./pages/board/BoardQrPage";
import BoardAccessPage from "./pages/board/BoardAccessPage";
import { ManageTournamentPage } from "./pages/tournament/ManageTournamentPage";
import { LocationPage } from "./pages/location/LocationPage";
import { BoardPage } from "./pages/newBoard/BoardPage";
import { ManageTournamentPlayers } from "./pages/tournament/ManageTournamentPlayers";

function App() {
  return (
    <AuthProvider>
      <Routes>
        {/* Public Routes */}
        <Route index element={<Navigate to="/dashboard" replace />} />
        <Route path="dashboard" element={<DashboardPage />} />
        <Route path="login" element={<LoginPage />} />
        <Route path="register" element={<RegisterPage />} />
        {/* Anonymous Board Scoring via QR/PIN */}
        <Route path="boards/:boardId" element={<BoardAccessPage />} />

        {/* Protected Routes (Require Authentication) */}
        <Route
          path="tournaments"
          element={
            <ProtectedRoute>
              <TournamentPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="tournaments/create"
          element={
            <ProtectedRoute>
              <CreateTournamentPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="tournaments/:id/manage"
          element={
            <ProtectedRoute>
              <ManageTournamentPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="tournaments/:id/players"
          element={
            <ProtectedRoute>
              <ManageTournamentPlayers />
            </ProtectedRoute>
          }
        />
        <Route
          path="players"
          element={
            <ProtectedRoute>
              <PlayerPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="locations"
          element={
            <ProtectedRoute>
              <LocationPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="boards"
          element={
            <ProtectedRoute>
              <BoardPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="boards/:boardId/qr"
          element={
            <ProtectedRoute>
              <BoardQrPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="settings"
          element={
            <ProtectedRoute>
              <SettingsPage />
            </ProtectedRoute>
          }
        />
      </Routes>
    </AuthProvider>
  );
}

export default App;
