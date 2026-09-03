export enum MatchStatus {
  Scheduled = 1,
  InProgress = 2,
  Completed = 3,
  Cancelled = 4,
}

export function matchStatusLabel(status: MatchStatus) {
  switch (status) {
    case MatchStatus.InProgress:
      return "Läuft";
    case MatchStatus.Completed:
      return "Beendet";
    case MatchStatus.Cancelled:
      return "Abgesagt";
    default:
      return "Geplant";
  }
}
