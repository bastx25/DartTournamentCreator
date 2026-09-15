import { useParams } from "react-router";
import Header from "../../components/Header";

export function ManageTournamentPlayers() {
  const { id } = useParams();
  return (
    <>
      <Header />

      <h2>Manage Players</h2>
    </>
  );
}
