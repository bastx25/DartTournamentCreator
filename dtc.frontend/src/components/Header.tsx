import { Link } from "react-router";

export default function Header() {
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
                            Dart<span className="text-primary">Forge</span>
                        </h1>

                        <p className="text-text-subtle hidden text-[10px] font-medium uppercase tracking-[0.2em] sm:block">
                            Tournament Creator
                        </p>
                    </div>
                </Link>

                {/* Navigation */}
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

                {/* Actions */}
                <div className="flex items-center gap-3">
                    <Link
                        to="/settings"
                        className="border-border bg-surface text-text-muted hover:border-text-subtle hover:bg-surface-hover hover:text-text hidden items-center gap-2 rounded-lg border px-4 py-2 text-sm font-semibold transition sm:flex"
                    >
                        <span>⚙️</span>
                        Einstellungen
                    </Link>

                    <Link
                        to="/tournaments/create"
                        className="bg-primary hover:bg-primary-hover shadow-primary/20 flex items-center gap-2 rounded-lg px-4 py-2 text-sm font-bold text-white shadow-lg transition active:scale-95"
                    >
                        <span className="text-lg">+</span>
                        <span className="hidden sm:inline">
                            Turnier erstellen
                        </span>
                    </Link>
                </div>
            </div>
        </header>
    );
}
