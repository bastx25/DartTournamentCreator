import { useState } from "react";
import { Link } from "react-router";

export default function Header() {
  const [menuOpen, setMenuOpen] = useState(false);

  return (
    <header className="border-border bg-background text-text border-b backdrop-blur">
      <div className="mx-auto flex h-16 max-w-7xl items-center justify-between px-4 sm:px-6 lg:px-8">
        {/* Logo */}
        <Link to="/" className="flex items-center gap-3">
          <div className="bg-primary shadow-primary/20 flex h-10 w-10 items-center justify-center rounded-xl shadow-lg">
            <span className="text-xl font-black">🎯</span>
          </div>

          <div>
            <h1 className="text-lg font-black tracking-tight">
              Landjugend <span className="text-primary">Lasberg</span>
            </h1>

            <p className="text-text-subtle hidden text-[10px] font-medium uppercase tracking-[0.2em] sm:block">
              Tournament Creator
            </p>
          </div>
        </Link>

        {/* Desktop Navigation */}
        <nav className="hidden items-center gap-1 md:flex">
          <Link
            to="/dashboard"
            className="text-text-muted hover:bg-surface hover:text-text rounded-lg px-4 py-2 text-sm font-medium transition"
          >
            Dashboard
          </Link>

          <Link
            to="/tournaments"
            className="bg-surface text-text rounded-lg px-4 py-2 text-sm font-medium"
          >
            Turniere
          </Link>

          <Link
            to="/players"
            className="text-text-muted hover:bg-surface hover:text-text rounded-lg px-4 py-2 text-sm font-medium transition"
          >
            Spieler
          </Link>
        </nav>

        {/* Desktop Actions */}
        <div className="hidden items-center gap-3 md:flex">
          <Link
            to="/settings"
            className="border-border bg-surface text-text-muted hover:border-text-subtle hover:bg-surface-hover hover:text-text flex items-center gap-2 rounded-lg border px-4 py-2 text-sm font-semibold transition"
          >
            <span>⚙️</span>
            Einstellungen
          </Link>

          <Link
            to="/tournaments/create"
            className="bg-primary hover:bg-primary-hover shadow-primary/20 flex items-center gap-2 rounded-lg px-4 py-2 text-sm font-bold text-white shadow-lg transition active:scale-95"
          >
            <span className="text-lg">+</span>
            Turnier erstellen
          </Link>
        </div>

        {/* Mobile Hamburger */}
        <button
          type="button"
          onClick={() => setMenuOpen(!menuOpen)}
          className="hover:bg-surface text-text flex h-10 w-10 items-center justify-center rounded-lg transition md:hidden"
          aria-label="Menü öffnen"
          aria-expanded={menuOpen}
        >
          <span className="text-2xl">{menuOpen ? "×" : "☰"}</span>
        </button>
      </div>

      {/* Mobile Menu */}
      {menuOpen && (
        <nav className="border-border bg-background border-t px-4 py-3 md:hidden">
          <div className="flex flex-col gap-1">
            <Link
              to="/dashboard"
              onClick={() => setMenuOpen(false)}
              className="text-text-muted hover:bg-surface hover:text-text rounded-lg px-4 py-3 text-sm font-medium transition"
            >
              Dashboard
            </Link>

            <Link
              to="/tournaments"
              onClick={() => setMenuOpen(false)}
              className="bg-surface text-text rounded-lg px-4 py-3 text-sm font-medium"
            >
              Turniere
            </Link>

            <Link
              to="/players"
              onClick={() => setMenuOpen(false)}
              className="text-text-muted hover:bg-surface hover:text-text rounded-lg px-4 py-3 text-sm font-medium transition"
            >
              Spieler
            </Link>

            <Link
              to="/settings"
              onClick={() => setMenuOpen(false)}
              className="text-text-muted hover:bg-surface hover:text-text rounded-lg px-4 py-3 text-sm font-medium transition"
            >
              ⚙️ Einstellungen
            </Link>

            <Link
              to="/tournaments/create"
              onClick={() => setMenuOpen(false)}
              className="bg-primary hover:bg-primary-hover mt-2 rounded-lg px-4 py-3 text-center text-sm font-bold text-white transition"
            >
              + Turnier erstellen
            </Link>
          </div>
        </nav>
      )}
    </header>
  );
}
