const COOKIE_NAME = "dtc_saved_tournaments";
const COOKIE_MAX_AGE_DAYS = 30;

/**
 * Reads a cookie value by name.
 */
export function getCookie(name: string): string | null {
  if (typeof document === "undefined") return null;

  const nameEQ = `${name}=`;
  const cookies = document.cookie.split(";");

  for (let cookie of cookies) {
    cookie = cookie.trim();
    if (cookie.indexOf(nameEQ) === 0) {
      const rawVal = cookie.substring(nameEQ.length);
      try {
        return decodeURIComponent(rawVal);
      } catch {
        return rawVal;
      }
    }
  }

  return null;
}

/**
 * Sets a cookie with standard options:
 * - SameSite=Lax
 * - Secure if HTTPS
 * - Path=/
 * - Configurable expiry (default 30 days)
 */
export function setCookie(name: string, value: string, days = COOKIE_MAX_AGE_DAYS): void {
  if (typeof document === "undefined") return;

  const date = new Date();
  date.setTime(date.getTime() + days * 24 * 60 * 60 * 1000);
  const expires = `; expires=${date.toUTCString()}`;

  const isSecure =
    typeof window !== "undefined" && window.location.protocol === "https:";
  const secureFlag = isSecure ? "; Secure" : "";

  document.cookie = `${name}=${encodeURIComponent(value)}${expires}; path=/; SameSite=Lax${secureFlag}`;
}

/**
 * Deletes a cookie by name.
 */
export function deleteCookie(name: string): void {
  if (typeof document === "undefined") return;
  document.cookie = `${name}=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/; SameSite=Lax`;
}

/**
 * Retrieves the saved tournament codes from the browser cookie.
 * Always returns an array of unique, trimmed, uppercase codes.
 */
export function getSavedTournamentCodes(): string[] {
  const raw = getCookie(COOKIE_NAME);
  if (!raw) return [];

  try {
    const parsed = JSON.parse(raw);
    if (Array.isArray(parsed)) {
      return Array.from(
        new Set(
          parsed
            .filter((item): item is string => typeof item === "string")
            .map((item) => item.trim().toUpperCase())
            .filter((item) => item.length > 0),
        ),
      );
    }
  } catch {
    // If not valid JSON, fallback to comma-separated format
    return Array.from(
      new Set(
        raw
          .split(",")
          .map((item) => item.trim().toUpperCase())
          .filter((item) => item.length > 0),
      ),
    );
  }

  return [];
}

/**
 * Persists an array of tournament codes into the browser cookie.
 */
export function saveTournamentCodes(codes: string[]): void {
  const cleaned = Array.from(
    new Set(
      codes
        .map((code) => code.trim().toUpperCase())
        .filter((code) => code.length > 0),
    ),
  );

  if (cleaned.length === 0) {
    deleteCookie(COOKIE_NAME);
  } else {
    setCookie(COOKIE_NAME, JSON.stringify(cleaned), COOKIE_MAX_AGE_DAYS);
  }
}

/**
 * Appends a tournament code to the saved codes cookie if not already present.
 * Returns the updated list of codes.
 */
export function addTournamentCodeToCookie(code: string): string[] {
  const normalized = code.trim().toUpperCase();
  if (!normalized) return getSavedTournamentCodes();

  const current = getSavedTournamentCodes();
  if (!current.includes(normalized)) {
    const updated = [...current, normalized];
    saveTournamentCodes(updated);
    return updated;
  }

  return current;
}

/**
 * Removes a tournament code from the saved codes cookie.
 * Returns the updated list of codes.
 */
export function removeTournamentCodeFromCookie(code: string): string[] {
  const normalized = code.trim().toUpperCase();
  const current = getSavedTournamentCodes();
  const updated = current.filter((item) => item !== normalized);
  saveTournamentCodes(updated);
  return updated;
}

/**
 * Removes all saved tournament codes from cookies.
 */
export function clearSavedTournamentCodes(): void {
  deleteCookie(COOKIE_NAME);
}
