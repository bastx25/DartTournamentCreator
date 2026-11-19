export interface PlayerNameSource {
  firstName?: string | null;
  lastName?: string | null;
  nickname?: string | null;
  displayName?: string | null;
}

/**
 * Returns a cleanly formatted display name for a player:
 * - "Max 'The Power' Mustermann"
 * - "Max Mustermann"
 * - "Max 'The Power'"
 * - "Max"
 * - Never returns "Max null" or "Max undefined"
 */
export function formatPlayerDisplayName(player?: PlayerNameSource | null, fallback = "TBD"): string {
  if (!player) return fallback;

  if (player.displayName && player.displayName.trim().length > 0) {
    return player.displayName.trim();
  }

  const parts: string[] = [];

  const first = player.firstName?.trim();
  const nick = player.nickname?.trim();
  const last = player.lastName?.trim();

  if (first) parts.push(first);
  if (nick) parts.push(`'${nick}'`);
  if (last) parts.push(last);

  return parts.length > 0 ? parts.join(" ") : fallback;
}
