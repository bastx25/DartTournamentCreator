export enum BraketStatus {
  Scheduled = 1,
  InProgress = 2,
  Completed = 3,
}

export function braketStatusLabel(status: BraketStatus) {
  switch (status) {
    case BraketStatus.InProgress:
      return "Läuft";
    case BraketStatus.Completed:
      return "Abgeschlossen";
    default:
      return "Geplant";
  }
}
