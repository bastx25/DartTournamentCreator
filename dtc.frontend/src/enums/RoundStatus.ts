export enum RoundStatus {
  Scheduled = 1,
  InProgress = 2,
  Completed = 3,
}

export function roundStatusLabel(status: RoundStatus) {
  switch (status) {
    case RoundStatus.InProgress:
      return "Läuft";
    case RoundStatus.Completed:
      return "Abgeschlossen";
    default:
      return "Geplant";
  }
}
