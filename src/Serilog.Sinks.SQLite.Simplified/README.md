Because Blazor wasm does not support multithreading, the Serilog.Sinks.SQLite package cannot be used
So I deleted all the code about threads, Simply save to database