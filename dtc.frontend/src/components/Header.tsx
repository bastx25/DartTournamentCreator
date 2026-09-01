import { Link } from "react-router";

export default function Header() {
    return (
        <header className="border-b border- bg-zinc-950/95 text-white backdrop-blur">
            <div className="mx-auto flex h-16 max-w-7xl items-center justify-between px-4 sm:px-6 lg:px-8">
                {/* Logo */}
                <Link to="/" className="flex items-center gap-3">
                    <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-red-600 shadow-lg shadow-red-600/20">
                        <span className="text-xl font-black">🎯</span>
                    </div>

                    <div>
                        <h1 className="text-lg font-black tracking-tight">
                            Dart<span className="text-red-500">Forge</span>
                        </h1>
                        <p className="hidden text-[10px] font-medium uppercase tracking-[0.2em] text-zinc-500 sm:block">
                            Tournament Creator
                        </p>
                    </div>
                </Link>

                {/* Navigation */}
                <nav className="hidden items-center gap-1 md:flex">
                    <Link
                        to="/dashboard"
                        className="rounded-lg px-4 py-2 text-sm font-medium text-zinc-400 transition hover:bg-zinc-900 hover:text-white"
                    >
                        Dashboard
                    </Link>

                    <Link
                        to="/tournaments"
                        className="rounded-lg bg-zinc-900 px-4 py-2 text-sm font-medium text-white"
                    >
                        Turniere
                    </Link>

                    <Link
                        to="/players"
                        className="rounded-lg px-4 py-2 text-sm font-medium text-zinc-400 transition hover:bg-zinc-900 hover:text-white"
                    >
                        Spieler
                    </Link>
                </nav>

                {/* Actions */}
                <div className="flex items-center gap-3">
                    <Link
                        to="/settings"
                        className="hidden items-center gap-2 rounded-lg border border-zinc-800 bg-zinc-900 px-4 py-2 text-sm font-semibold text-zinc-200 transition hover:border-zinc-700 hover:bg-zinc-800 sm:flex"
                    >
                        <span>⚙️</span>
                        Einstellungen
                    </Link>

                    <Link
                        to="/tournaments/create"
                        className="flex items-center gap-2 rounded-lg bg-red-600 px-4 py-2 text-sm font-bold text-white shadow-lg shadow-red-600/20 transition hover:bg-red-500 active:scale-95"
                    >
                        <span className="text-lg">+</span>
                        <span className="hidden sm:inline">Turnier erstellen</span>
                    </Link>
                </div>
            </div>
        </header>
    );
}
