import { Link } from "react-router";

export default function HeaderEmpty() {
  return (
    <header className="border-border bg-backgbraket text-text border-b backdrop-blur">
      <div className="mx-auto flex h-16 max-w-7xl items-center justify-between px-4 sm:px-6 lg:px-8">
        {/* Logo */}
        <Link to="/" className="flex items-center gap-3">
          <div className="bg-primary shadow-primary/20 flex h-10 w-10 items-center justify-center braketed-xl shadow-lg">
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
      </div>
    </header>
  );
}
