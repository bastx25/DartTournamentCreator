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
