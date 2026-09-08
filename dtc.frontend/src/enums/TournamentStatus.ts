export enum TournamentStatus {
  Draft = 1,
  Scheduled = 2,
  InProgress = 3,
  Completed = 4,
  Cancelled = 5,
}

export function tournamentStatusLabel(status: TournamentStatus) {
  switch (status) {
    case TournamentStatus.Scheduled:
      return "Geplant";
    case TournamentStatus.InProgress:
      return "Laufend";
    default:
      return "Aktiv";
  }
}

export const tournamentStatusClasses: Record<TournamentStatus, string> = {
  [TournamentStatus.Draft]: "bg-gray-100 text-gray-700",
  [TournamentStatus.Scheduled]: "bg-blue-50 text-blue-700",
  [TournamentStatus.InProgress]: "bg-green-50 text-green-700",
  [TournamentStatus.Completed]: "bg-purple-50 text-purple-700",
  [TournamentStatus.Cancelled]: "bg-red-50 text-red-700",
};

export const tournamentStatusLabels: Record<TournamentStatus, string> = {
  [TournamentStatus.Draft]: "Entwurf",
  [TournamentStatus.Scheduled]: "Geplant",
  [TournamentStatus.InProgress]: "Laufend",
  [TournamentStatus.Completed]: "Abgeschlossen",
  [TournamentStatus.Cancelled]: "Abgebrochen",
};
