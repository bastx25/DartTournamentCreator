import { useState } from "react";
import { Link, useNavigate } from "react-router";
import { useAuth } from "../context/AuthContext";

export default function Header() {
  const [menuOpen, setMenuOpen] = useState(false);
  const { isAuthenticated, user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async () => {
    setMenuOpen(false);
    await logout();
    navigate("/login");
  };

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

          {isAuthenticated && (
            <>
              <Link
                to="/tournaments"
                className="text-text-muted hover:bg-surface hover:text-text rounded-lg px-4 py-2 text-sm font-medium transition"
              >
                Turniere
              </Link>

              <Link
                to="/players"
                className="text-text-muted hover:bg-surface hover:text-text rounded-lg px-4 py-2 text-sm font-medium transition"
              >
                Spieler
              </Link>

              <Link
                to="/locations"
                className="text-text-muted hover:bg-surface hover:text-text rounded-lg px-4 py-2 text-sm font-medium transition"
              >
                Locations
              </Link>

              <Link
                to="/boards"
                className="text-text-muted hover:bg-surface hover:text-text rounded-lg px-4 py-2 text-sm font-medium transition"
              >
                Boards
              </Link>
            </>
          )}
        </nav>

        {/* Desktop Actions */}
        <div className="hidden items-center gap-3 md:flex">
          {isAuthenticated ? (
            <>
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

              {/* User indicator */}
              <div className="flex items-center gap-2 border-l border-border pl-3">
                <span className="text-xs text-text-subtle max-w-[140px] truncate" title={user?.email}>
                  {user?.firstName ? `Hallo, ${user.firstName}` : user?.email}
                </span>
                <button
                  type="button"
                  onClick={handleLogout}
                  className="border-border bg-surface text-text-muted hover:bg-surface-hover hover:text-text rounded-lg border px-3 py-1.5 text-xs font-medium transition"
                >
                  Abmelden
                </button>
              </div>
            </>
          ) : (
            <>
              <Link
                to="/login"
                className="border-border bg-surface text-text-muted hover:border-text-subtle hover:bg-surface-hover hover:text-text rounded-lg border px-4 py-2 text-sm font-semibold transition"
              >
                Anmelden
              </Link>
              <Link
                to="/register"
                className="bg-primary hover:bg-primary-hover shadow-primary/20 flex items-center gap-2 rounded-lg px-4 py-2 text-sm font-bold text-white shadow-lg transition active:scale-95"
              >
                Registrieren
              </Link>
            </>
          )}
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

            {isAuthenticated ? (
              <>
                <Link
                  to="/tournaments"
                  onClick={() => setMenuOpen(false)}
                  className="text-text-muted hover:bg-surface hover:text-text rounded-lg px-4 py-3 text-sm font-medium transition"
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
                  to="/locations"
                  onClick={() => setMenuOpen(false)}
                  className="text-text-muted hover:bg-surface hover:text-text rounded-lg px-4 py-3 text-sm font-medium transition"
                >
                  Locations
                </Link>

                <Link
                  to="/boards"
                  onClick={() => setMenuOpen(false)}
                  className="text-text-muted hover:bg-surface hover:text-text rounded-lg px-4 py-3 text-sm font-medium transition"
                >
                  Boards
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

                <div className="mt-2 border-t border-border pt-3 flex items-center justify-between">
                  <span className="text-xs text-text-subtle truncate">
                    {user?.email}
                  </span>
                  <button
                    type="button"
                    onClick={handleLogout}
                    className="border-border bg-surface text-text-muted hover:bg-surface-hover hover:text-text rounded-lg border px-3 py-1.5 text-xs font-medium"
                  >
                    Abmelden
                  </button>
                </div>
              </>
            ) : (
              <div className="mt-2 flex flex-col gap-2 border-t border-border pt-3">
                <Link
                  to="/login"
                  onClick={() => setMenuOpen(false)}
                  className="border-border bg-surface text-text-muted hover:text-text rounded-lg border px-4 py-3 text-center text-sm font-semibold"
                >
                  Anmelden
                </Link>
                <Link
                  to="/register"
                  onClick={() => setMenuOpen(false)}
                  className="bg-primary hover:bg-primary-hover rounded-lg px-4 py-3 text-center text-sm font-bold text-white"
                >
                  Registrieren
                </Link>
              </div>
            )}
          </div>
        </nav>
      )}
    </header>
  );
}
