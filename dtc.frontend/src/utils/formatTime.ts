const timeFormatter = new Intl.DateTimeFormat("de-DE", {
  hour: "2-digit",
  minute: "2-digit",
});

export function formatTime(value: string) {
  return timeFormatter.format(new Date(value));
}
