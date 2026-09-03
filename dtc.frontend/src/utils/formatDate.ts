export function formatDate(value: string) {
  return dateFormatter.format(new Date(value));
}

const dateFormatter = new Intl.DateTimeFormat("de-DE", {
  dateStyle: "medium",
  timeStyle: "short",
});
