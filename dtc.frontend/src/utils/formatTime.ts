const timeFormatter = new Intl.DateTimeFormat("de-DE", {
  hour: "2-digit",
  minute: "2-digit",
});

export function formatTime(value: string | null) {
  return value != null ? timeFormatter.format(new Date(value)) : null;
}
