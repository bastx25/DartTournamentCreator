import { useNavigate } from "react-router";
import Header from "../../components/Header";

import { TournamentList } from "./TournamentList";
import { useTournaments } from "../../hooks/useTournaments";

export function TournamentPage() {
  const navigate = useNavigate();
  const { tournaments, loading, error } = useTournaments();

  const handleManageTournament = (tournamentId: number) => {
    // Die Verwaltungsseite wird später ergänzt.
    navigate(`/tournaments/${tournamentId}/manage`);
  };

  return (
    <>
      <Header />

      <main className="min-h-screen bg-gray-100 px-4 py-8 sm:px-6 lg:px-8">
        {loading && (
          <div className="flex min-h-75 items-center justify-center">
            <p className="text-sm text-gray-400">Turniere werden geladen...</p>
          </div>
        )}

        {error && (
          <div className="mx-auto max-w-7xl rounded-lg border border-red-500/20 bg-red-500/10 p-4">
            <p className="text-sm text-red-600">{error}</p>
          </div>
        )}

        {!loading && !error && (
          <div className="mx-auto max-w-7xl">
            <TournamentList
              tournaments={tournaments}
              onManageTournament={(tournament) =>
                handleManageTournament(tournament.id)
              }
            />
          </div>
        )}
      </main>
    </>
  );
}
