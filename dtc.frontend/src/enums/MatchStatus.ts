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

export function statusClass(status: MatchStatus) {
  switch (status) {
    case MatchStatus.InProgress:
      return "border-amber-200 bg-amber-50 text-amber-700";
    case MatchStatus.Completed:
      return "border-green-200 bg-green-50 text-green-700";
    case MatchStatus.Cancelled:
      return "border-red-200 bg-red-50 text-red-700";
    default:
      return "border-gray-200 bg-gray-50 text-gray-600";
  }
}
