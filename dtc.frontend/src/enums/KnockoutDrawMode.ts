export enum KnockoutDrawMode {
  Random = 1,
  RankedSeeding = 2,
}

export const knockoutDrawModeLabel = (mode?: KnockoutDrawMode): string => {
  switch (mode) {
    case KnockoutDrawMode.RankedSeeding:
      return "Beste gegen schlechteste Platzierung (Setzliste)";
    case KnockoutDrawMode.Random:
    default:
      return "Zufällige Auslosung";
  }
};
